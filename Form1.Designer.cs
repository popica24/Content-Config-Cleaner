namespace EpromSolution
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OpenInput = new System.Windows.Forms.OpenFileDialog();
            this.OpenBtn = new System.Windows.Forms.Button();
            this.InputPathLabel = new System.Windows.Forms.Label();
            this.StartBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.FinalStatus = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OpenInput
            // 
            this.OpenInput.FileName = "openFileDialog1";
            // 
            // OpenBtn
            // 
            this.OpenBtn.Location = new System.Drawing.Point(6, 19);
            this.OpenBtn.Name = "OpenBtn";
            this.OpenBtn.Size = new System.Drawing.Size(188, 27);
            this.OpenBtn.TabIndex = 0;
            this.OpenBtn.Text = "Open";
            this.OpenBtn.UseVisualStyleBackColor = true;
            this.OpenBtn.Click += new System.EventHandler(this.OpenBtn_Click);
            // 
            // InputPathLabel
            // 
            this.InputPathLabel.AutoSize = true;
            this.InputPathLabel.Location = new System.Drawing.Point(258, 419);
            this.InputPathLabel.Name = "InputPathLabel";
            this.InputPathLabel.Size = new System.Drawing.Size(0, 13);
            this.InputPathLabel.TabIndex = 1;
            // 
            // StartBtn
            // 
            this.StartBtn.Location = new System.Drawing.Point(6, 68);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(188, 26);
            this.StartBtn.TabIndex = 2;
            this.StartBtn.Text = "Start";
            this.StartBtn.UseVisualStyleBackColor = true;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.StartBtn);
            this.groupBox1.Controls.Add(this.OpenBtn);
            this.groupBox1.Location = new System.Drawing.Point(12, 338);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(218, 419);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Input:";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(539, 320);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // FinalStatus
            // 
            this.FinalStatus.AutoSize = true;
            this.FinalStatus.Location = new System.Drawing.Point(223, 357);
            this.FinalStatus.Name = "FinalStatus";
            this.FinalStatus.Size = new System.Drawing.Size(0, 13);
            this.FinalStatus.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 450);
            this.Controls.Add(this.FinalStatus);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.InputPathLabel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog OpenInput;
        private System.Windows.Forms.Button OpenBtn;
        private System.Windows.Forms.Label InputPathLabel;
        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label FinalStatus;
    }
}

