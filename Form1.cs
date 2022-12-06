using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IniParser;
using MoreLinq;
using System.Text.RegularExpressions;

namespace EpromSolution
{
    public partial class Form1 : Form
    {
        FileIniDataParser parser;
        public string InputPath;
        public List<string> Roots = new List<string>();
        public List<string> FoldersToDelete = new List<string>();
        public List<string> Logs = new List<string>();
        public int trimmed = 0;
        public int pass = 0;
        public int total = 0;
        public Form1()
        {
            InitializeComponent();
            parser = new FileIniDataParser();
            StartBtn.Enabled = false;
            InputPathLabel.TextChanged += (s, args) => {
                StartBtn.Enabled = true;
            };
            OpenInput.Filter = "Text|*.txt|All|*.*";

        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            FinalStatus.Text = "";
            try
            {
                OpenInput.ShowDialog();
                InputPath = OpenInput.FileName;
                InputPathLabel.Text = InputPath;

            }
            catch (Exception ex)
            {
                InputPath = null;
                richTextBox1.Text += ex.Message;
                StartBtn.Enabled = false;
            }
            var RootFolder = String.Empty;

            using (var fs = new FileStream(InputPath, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine().Trim();
                        if (line == "")
                        {
                            total++;
                            RootFolder = String.Empty;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(RootFolder))
                            {
                                RootFolder = Directory.GetParent(line).ToString();
                            }
                            else
                            {

                                try
                                {
                                    EraseFile(line);
                                }
                                catch (Exception ex)
                                {
                                    richTextBox1.Text += "Error Trimmig " + line + " --- " + ex.Message + '\n';
                                    Logs.Add("Error Trimmig " + line + " --- " + ex.Message + '\n');

                                }

                            }
                        }
                    }
                }
                FinalStatus.Text += "Counted" + total.ToString() + " batches";
            }
        }
        private void StartBtn_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            total = 0;
            FinalStatus.Text = "";
            var RootFolder = string.Empty;
            List<string> DependentFolders = new List<string>();
            using (var fs = new FileStream(InputPath, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine().Trim();
                        if (line == "")
                        {
                            total++;
                            try
                            {
                                ProcessBatch(RootFolder, DependentFolders);
                                pass++;
                            }
                            catch (Exception ex)
                            {
                                richTextBox1.Text += "Error processing batch at Root " + RootFolder + " --- " + ex.Message + '\n';
                                var s = String.Empty;
                                foreach (var x in DependentFolders)
                                    s += x.ToString() + " | ";
                                Logs.Add("Error processing batch at Root " + RootFolder + " --- " + s + '\n'+ex.Message+'\n');
                            }
                            finally
                            {
                                RootFolder = String.Empty;
                                DependentFolders.Clear();
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(RootFolder))
                            {
                                RootFolder = Directory.GetParent(line).ToString();
                                if (!Roots.Contains(RootFolder)) Roots.Add(RootFolder);
                            }
                            else

                                DependentFolders.Add(line);
                        }
                    }
                }
            }
            var ftd = FoldersToDelete;
            var r = Roots;
            var newList = FoldersToDelete.Except(Roots).ToList();
            for (int i = 0; i < newList.Count; i++)
            {
                try
                {
                    Directory.Delete(newList[i], true);
                }
                catch (Exception ex)
                {
                    richTextBox1.Text += "Error Deleting Folder " + newList[i] + '\n';
                    Logs.Add("Error Deleting Folder " + newList[i] + '\n');
                }
            }
            try
            {
                RefactorBatch(Roots);
            }
            catch (Exception ex)
            {

            }
            FinalStatus.Text +=
              "Successfully processed " + pass.ToString() + " of " + total.ToString() + "\n";
            File.WriteAllLines("EPROMLOGS.txt", Logs);

        }
        #region
        private string GetVersionName(string path)
        {
            var Folder = Directory.GetParent(path).ToString();
            var FileName = Path.GetFileName(path).ToString();
            var Digits = Regex.Match(FileName, @"\d+").Value.ToString();
            var DependentFolderContent = parser.ReadFile(Path.Combine(Folder, "contents.ini"));
            var DependentFolderString = DependentFolderContent.ToString();
            List<string> list = new List<string>(
              Regex.Split(DependentFolderString, Environment.NewLine)
            );

            if (Convert.ToInt32(Digits) == 1)
            {
                return list.FirstOrDefault(s => s.Contains("VersionName"))
                  .Substring(
                    list.FirstOrDefault(s => s.Contains("VersionName")).LastIndexOf('=') + 1
                  )
                  .Trim();
            }
            string result = list.FirstOrDefault(s => s.Contains("VersionName_v" + Digits));
            if (string.IsNullOrEmpty(result))
                return null;

            return result.Substring(result.LastIndexOf('=') + 1).Trim();
        }
        private void EraseFile(string path)
        {
            File.Delete(path);

            var ParentFolder = Directory.GetParent(path).ToString();

            var Digits = Regex.Match(Path.GetFileName(path).ToString(), @"\d+").Value.ToString();
            var DependentFolderContent = parser
              .ReadFile(Path.Combine(ParentFolder, "contents.ini"))
              .ToString();

            List<string> list = new List<string>(
              Regex.Split(DependentFolderContent, Environment.NewLine)
            );

            if (Convert.ToInt32(Digits) == 1)
            {
                var IndexToRemoveVName = list.FirstOrDefault(s => s.Contains("VersionName ="));

                list.RemoveAt(list.IndexOf(IndexToRemoveVName));
            }
            else
            {
                var IndexToRemoveVName = list.FirstOrDefault(s => s.Contains("VersionName_v" + Digits));

                list.RemoveAt(list.IndexOf(IndexToRemoveVName));

            }
            if (Convert.ToInt32(Digits) == 1)
            {
                var IndexToRemoveFName = list.FirstOrDefault(s => s.Contains("Filename ="));

                list.RemoveAt(list.IndexOf(IndexToRemoveFName));
            }
            else
            {
                var IndexToRemoveFName = list.FirstOrDefault(s => s.Contains("Filename_v" + Digits));
                list.RemoveAt(list.IndexOf(IndexToRemoveFName));
            }
            File.WriteAllLines(Path.Combine(ParentFolder, "contents.ini"), list);

        }
        private string GetVeryNextAvalabileName(string name)
        {
            var FileTextOnly = Regex.Match(name, @"^[^0-9]*").Value;
            var FileNumbersOnly = Regex.Match(name, @"\d+").Value;

            return FileTextOnly + (Convert.ToInt32(FileNumbersOnly) + 1).ToString() + ".bin";
        }
        private string GetDigit(string Name)
        {
            return Regex.Match(Name, @"\d+").Value;
        }
        private void InsertInFolder(string binFile, string rootFolder)
        {
            var files = Directory.EnumerateFiles(rootFolder, "*.bin");
            var min = Path.GetFileName(files.First());
            foreach (var f in files)
            {
                if (Convert.ToInt32(GetDigit(Path.GetFileName(f))) > Convert.ToInt32(GetDigit(min)))
                    min = Path.GetFileName(f);
            }
            var Name = GetVeryNextAvalabileName(min);
            File.Move(binFile, Path.Combine(rootFolder, Name));
        }
        private void InsertInContents(List<string> VersionNames, string path)
        {
            var DependentFolderContent = parser.ReadFile(path);
            var DependentFolderString = DependentFolderContent.ToString();
            List<string> list = new List<string>(
              Regex.Split(DependentFolderString, Environment.NewLine)
            );
           
            var LastIndexOfVersionName = list.LastOrDefault(s => s.Contains("VersionName_v"));
            int LastInfdexOfVersionNameDigits = Convert.ToInt32(
              Regex.Match(LastIndexOfVersionName, @"\d+").Value
            );
            var VNIndex = list.IndexOf(LastIndexOfVersionName) + 1;
            foreach (var entry in VersionNames)
            {
                LastInfdexOfVersionNameDigits += 1;
                list.Insert(
                  VNIndex,
                  "VersionName_v" + LastInfdexOfVersionNameDigits.ToString() + " = " + entry
                );
                VNIndex += 1;
            }
            var LastIndexOfFileName = list.LastOrDefault(s => s.Contains("Filename_v"));
            int LastIndexOfFileNameDigits = Convert.ToInt32(
              Regex.Match(LastIndexOfFileName, @"\d+").Value
            );
            var FNIndex = list.IndexOf(LastIndexOfFileName) + 1;
            for (int i = 0; i < VersionNames.Count; i++)
            {
                LastIndexOfFileNameDigits++;
                list.Insert(
                  FNIndex,
                  "Filename_v" +
                  LastIndexOfFileNameDigits.ToString() +
                  " = Eprom" +
                  LastIndexOfFileNameDigits.ToString() +
                  ".bin"
                );
                FNIndex++;
            }
            
            File.WriteAllLines(path, list);
        }
        private string RenameVersionName(string line, int i)
        {
            var value = line.Substring(line.LastIndexOf("=")+1).Trim();
            if (i == 1) return "VersionName = " + value;
            else return "VersionName_v" + i.ToString() + " = " + value;
        }
        private string RenameFileName(string line, int i)
        {
            var value = line.Substring(line.LastIndexOf("=")).Trim();
            if (i == 1) return "Filename = Eprom1.bin";
            else return "Filename_v" + i.ToString() + " = Eprom" + i.ToString() + ".bin";
        }
        private void RefactorBatch(List<string> roots)
        {

            foreach (var Folder in roots)
            {
                int VnameI = 1;
                int FnameI = 1;
                var BinFiles = Directory.GetFiles(Folder, "*.bin").OrderBy(BinFile => Convert.ToInt32(GetDigit(Path.GetFileName(BinFile))));
                int i = 1;
                foreach (var BinFile in BinFiles)
                {
                    var oldName = Path.Combine(Directory.GetParent(BinFile).ToString(), Path.GetFileName(BinFile));
                    var newName = Path.Combine(Directory.GetParent(BinFile).ToString(), "Eprom" + i.ToString() + ".bin");
                    if (!oldName.Equals(newName)) File.Move(oldName, newName);
                    i++;
                }
                var DependentFolderContent = parser.ReadFile(Path.Combine(Folder, "contents.ini"));
                var DependentFolderString = DependentFolderContent.ToString();
                List<string> list = new List<string>(
                  Regex.Split(DependentFolderString, Environment.NewLine)
                );
                for (int q = 0; q < list.Count; q++)
                {
                    if (list[q].Contains("VersionName"))
                    {
                        list[q] = RenameVersionName(list[q], VnameI);
                        VnameI++;
                    }
                    if (list[q].Contains("Filename"))
                    {
                        list[q] = RenameFileName(list[q], FnameI);
                        FnameI++;
                    }
                }
                var NumVersions = Directory.GetFiles(Folder, "*.bin").Length;
                list[list.IndexOf(list.LastOrDefault(s => s.Contains("NumVersions")))] =
                  "NumVersions = " + NumVersions;
                File.WriteAllLines(Path.Combine(Folder, "contents.ini"), list);
            }

        }
        #endregion
        private void ProcessBatch(string rootFolder, List<string> DependentFiles)
        {
            List<string> Quoran = new List<string>();
            foreach (var F in DependentFiles)
            {
                var ParentF = Directory.GetParent(F).ToString();
              
                if (ParentF.Equals(rootFolder)) continue;
                foreach (var BinFile in Directory.GetFiles(Directory.GetParent(F).ToString(), "*.bin").OrderBy(BinFile => Convert.ToInt32(GetDigit(Path.GetFileName(BinFile)))))
                {
                    var x = BinFile;
                    var target = GetVersionName(BinFile);
                    if (string.IsNullOrEmpty(target))
                        continue;
                    else
                    {
                        Quoran.Add(target);
                        InsertInFolder(BinFile, rootFolder);

                    }
                }
                InsertInContents(Quoran, Path.Combine(rootFolder, "contents.ini"));

                Quoran.Clear();

                if (!FoldersToDelete.Any(P => P.Equals(Directory.GetParent(F))))
                    FoldersToDelete.Add(Directory.GetParent(F).ToString());
            }

            richTextBox1.Text += rootFolder + "-----------" + pass + "\n";
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    }
}