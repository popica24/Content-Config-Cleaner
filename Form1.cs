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
           /* var RootFolder = String.Empty;

            using (var fs = new FileStream(InputPath, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine().Trim();
                        if (line == "")
                        {
                         
                            RootFolder = String.Empty;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(RootFolder))
                            {
                                RootFolder = Directory.GetParent(line).ToString();
                                Roots.Add(RootFolder);
                            }
                            else
                            {

                                try
                                {
                                    EraseFile(line);
                                 }
                                catch (Exception ex)
                                {
                                    richTextBox1.Text += "Error --- " + Directory.GetParent(line).ToString().Substring(Directory.GetParent(line).ToString().LastIndexOf(@"\") + 1) + " --- " + ex.Message + '\n';
                                    Logs.Add("Error Trimmig " + ex.Message + '\n');
                                }

                            }
                        }
                    }
                }
                FinalStatus.Text += "Counted " + Roots.Count.ToString() + " batches";
            }
*/
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            var _temp = new List<string>();
            richTextBox1.Text = "";
            total = 0;
            FinalStatus.Text = "";
            var RootFolder = String.Empty;
            using (var fs = new FileStream(InputPath, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine().Trim();
                        if (line == ""&&!string.IsNullOrEmpty(RootFolder))
                        {
                            foreach (var s in _temp)
                            {
                                try
                                {
                                    EraseFile(s);
                                }
                                catch (Exception ex) 
                                { }
                            }
                            foreach (var s in _temp)
                            {
                                if (!Directory.GetParent(s).ToString().Equals(RootFolder)&&Directory.Exists(Directory.GetParent(s).ToString()))
                                {
                                    try
                                    {
                                        ProcessBatch(RootFolder, Directory.GetParent(s).ToString());
                                        richTextBox1.Text +=Directory.GetParent(s).ToString().Substring(Directory.GetParent(s).ToString().LastIndexOf(@"\") + 1).ToString().Trim()+" -->>" +RootFolder.Substring(RootFolder.LastIndexOf(@"\") + 1).ToString().Trim()+ " ✓" + '\n';
                                    }
                                    catch (Exception ex) {
                                        Logs.Add(ex.Message + "\n");
                                    }
                                }
                            }
                            RootFolder = String.Empty;
                            _temp.Clear();
                      
                        }
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (Directory.Exists(Directory.GetParent(line).ToString()))
                            {
                                if (string.IsNullOrEmpty(RootFolder))
                                {
                                    RootFolder = Directory.GetParent(line).ToString();
                                    if (!Roots.Contains(RootFolder))
                                    {
                                        Roots.Add(RootFolder);
                                    }
                                }
                                else
                                {
                                    _temp.Add(line);
                                }
                            }
                        }

                    }
                }
            }

            /*
             string line = sr.ReadLine().Trim();
                            if (line.Equals(""))
                            {

                                foreach(var s in _temp)
                                {
                                    try { ProcessBatch(RootFolder, s); }
                                    catch(Exception ex) { }
                                }

                                _temp.Clear();
                                total++;
                                RootFolder = String.Empty;
                                continue;
                            }
                            if (String.IsNullOrEmpty(RootFolder) && !line.Equals(""))
                            {
                                RootFolder = Directory.GetParent(line).ToString();
                                Roots.Add(RootFolder);
                            }
                            if (!Directory.GetParent(line).ToString().Equals(RootFolder))
                                try
                                {
                                    EraseFile(line);
                                    _temp.Add(Directory.GetParent(line).ToString());
                                }
                                catch (Exception ex)
                                {
                                    richTextBox1.Text += "Error --- " + Directory.GetParent(line).ToString().Substring(Directory.GetParent(line).ToString().LastIndexOf(@"\") + 1)+" --- "+ex.Message;
                                    Logs.Add("Error Processing " + ex.Message + '\n');
                                }
            */
            richTextBox1.Text += "\nRenaming Folders, please wait...";
            foreach(var R in Roots)
            {

              
                try {
                    if (Directory.GetFiles(R, "*.bin").Length == 0)
                    {
                        Directory.Delete(R, true);
                    }
                   RefactorBatch(R);
                    }
              catch(Exception ex) { }
            }
            FinalStatus.Text = "Operation Completed";
            File.WriteAllLines("EPROMLOGS.txt", Logs);

        }
        #region
        private void RefactorBatch(string path)
        {
            var x = Directory.GetFiles(path, "*.bin").Length;
            var i = 1;
            foreach (var F in Directory.GetFiles(path, "*.bin"))
            {
                var oldName = F;
                var newName = Path.Combine(path, "Eprom" + i + ".bin");
                if (!File.Exists(newName)) File.Move(oldName, newName);
                i++;
            }
            var _temp = parser.ReadFile(Path.Combine(path, "contents.ini")).ToString();
            List<string> Contents = new List<string>(
              Regex.Split(_temp, Environment.NewLine)
            );
             var NewList = Contents.Where(line => !line.Contains("Filename")).ToList();
            var j = 1;
            foreach (var l in NewList.Where(l => l.Contains("VersionName")).ToList())
            {
                NewList[NewList.IndexOf(l)] = ProcessVersionName(l, j);
                j++;
            }
            var FileNameIndex = NewList.IndexOf("[File1]") + 1;
            for (int k = 1; k <= x; k++)
            {
                if (k == 1)
                {
                    NewList.Insert(FileNameIndex, "Filename = Eprom1.bin");
                    FileNameIndex++;
                }
                else
                {
                    NewList.Insert(FileNameIndex, "Filename_v" + k.ToString() + " = Eprom" + k.ToString() + ".bin");
                    FileNameIndex++;
                }
            }
            var NV = NewList.IndexOf(NewList.LastOrDefault(s => s.Contains("NumVersions")));
            var v = "NumVersions = " + x.ToString();
            NewList[NV] = v;
            File.WriteAllLines(Path.Combine(path, "contents.ini"), NewList);

        }
        private void EraseFile(string path)
        {
            var ParentFolder = Directory.GetParent(path).ToString();

            var Digits = Regex.Match(Path.GetFileName(path).ToString(), @"\d+").Value.ToString();
            var DependentFolderContent = parser
              .ReadFile(Path.Combine(ParentFolder, "contents.ini"))
              .ToString();

            List<string> list = new List<string>(
              Regex.Split(DependentFolderContent, Environment.NewLine)
            );
            File.Delete(path);

           

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
            
            File.WriteAllLines(Path.Combine(ParentFolder, "contents.ini"), list);

        } //Deletes VersionName, Filename and File
        private int GetDigit(string Name)
        {
            return Convert.ToInt32(Regex.Match(Name, @"\d+").Value);
        }   //Returns the digit from a string
        private string ProcessVersionName(string VersionName, int I) //VersionName_vx=xxxxx -->>> VersionName_vi=xxxxx
        {
            var Value = VersionName.Substring(VersionName.IndexOf("=") + 1).Trim();
            if (I == 1) return "VersionName = " + Value;

            var x = "VersionName_v" + I.ToString() + " = " + Value;
            return x;

        }
     
        private void InsertInContents(string Root, List<string> RootContents, List<string> Contents, string Name)
        {
            string LastVersionName = RootContents.LastOrDefault(s => s.Contains("VersionName")); //VersionName_vx = xxxxx
            int LastVersionNameIndex = RootContents.IndexOf(LastVersionName);
            int I = GetDigit(LastVersionName) + 1;
           
            foreach (var VersionName in Contents.Where(VersionName => VersionName.Contains("VersionName")))
            {

                LastVersionNameIndex++;
                RootContents.Insert(LastVersionNameIndex, ProcessVersionName(VersionName, I) + "   --->>>  " + Name.Substring(Name.LastIndexOf(@"\")+1));
    
                      I++;

            }
           

            File.WriteAllLines(Path.Combine(Root, "contents.ini"), RootContents);
        } //Inserts in contents.ini the Filename, and versionname and WRITES it.
        private void CopyToFolder(string Root, string Child)
        {
            int i = 1;
            foreach (var F in Directory.GetFiles(Root, "*.bin").OrderBy(F => Convert.ToInt32(GetDigit(Path.GetFileName(F)))))
            {
                var oldName = Path.Combine(Directory.GetParent(F).ToString(), Path.GetFileName(F));
                var newName = Path.Combine(Directory.GetParent(F).ToString(), "Eprom" + i.ToString() + ".bin");
                if (!oldName.Equals(newName)) File.Move(oldName, newName);
                i++;
            }

            foreach (var F in Directory.GetFiles(Child, "*.bin").OrderBy(F => Convert.ToInt32(GetDigit(Path.GetFileName(F)))))
            {
                var oldName = Path.Combine(Directory.GetParent(F).ToString(), Path.GetFileName(F));
                var newName = Path.Combine(Root, "Eprom" + i.ToString() + ".bin");
                File.Move(oldName, newName);
                i++;
            }
        }      //Inserts in the Root folder, the child files on the next avalabile itterators.
        #endregion 
        private void ProcessBatch(string Root, string Child) {
            if (!(Directory.GetFiles(Child,"*.bin").Length==0))
            {
                var _temp = parser.ReadFile(Path.Combine(Child, "contents.ini")).ToString();
                List<string> ChildContents = new List<string>(
                  Regex.Split(_temp, Environment.NewLine)
                );

                _temp = parser.ReadFile(Path.Combine(Root, "contents.ini")).ToString();
                List<string> RootContents = new List<string>(
                  Regex.Split(_temp, Environment.NewLine)
                );
                _temp = String.Empty;
                InsertInContents(Root, RootContents, ChildContents, Child);
                CopyToFolder(Root,Child);
              //  if(!Roots.Contains(Child))
                Directory.Delete(Child, true);
                
            }
        }     //Does the job

    private void richTextBox1_TextChanged(object sender, EventArgs e)
    {
        richTextBox1.SelectionStart = richTextBox1.Text.Length;
        richTextBox1.ScrollToCaret();
    }
}}
