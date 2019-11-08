namespace StarrForensics
{
    partial class Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.selectFileButton = new System.Windows.Forms.Button();
            this.fileProgressBar = new System.Windows.Forms.ProgressBar();
            this.scanFilesButton = new System.Windows.Forms.Button();
            this.verifyFilesBtn = new System.Windows.Forms.Button();
            // 
            // selectFileButton
            // 
            this.selectFileButton.Location = new System.Drawing.Point(8, 47);
            this.selectFileButton.Name = "selectFileButton";
            this.selectFileButton.Size = new System.Drawing.Size(121, 23);
            this.selectFileButton.TabIndex = 0;
            this.selectFileButton.Text = "Select File System";
            this.selectFileButton.UseVisualStyleBackColor = true;
            this.selectFileButton.Click += new System.EventHandler(this.selectFileButton_Click);
            // 
            // fileProgressBar
            // 
            this.fileProgressBar.Location = new System.Drawing.Point(8, 8);
            this.fileProgressBar.Name = "fileProgressBar";
            this.fileProgressBar.Size = new System.Drawing.Size(375, 23);
            this.fileProgressBar.TabIndex = 1;
            // 
            // scanFilesButton
            // 
            this.scanFilesButton.Location = new System.Drawing.Point(135, 47);
            this.scanFilesButton.Name = "scanFilesButton";
            this.scanFilesButton.Size = new System.Drawing.Size(121, 23);
            this.scanFilesButton.TabIndex = 2;
            this.scanFilesButton.Text = "Scan File System";
            this.scanFilesButton.UseVisualStyleBackColor = true;
            this.scanFilesButton.Click += new System.EventHandler(this.scanFilesButton_Click);
            // 
            // verifyFilesBtn
            // 
            this.verifyFilesBtn.Location = new System.Drawing.Point(262, 47);
            this.verifyFilesBtn.Name = "verifyFilesBtn";
            this.verifyFilesBtn.Size = new System.Drawing.Size(121, 23);
            this.verifyFilesBtn.TabIndex = 3;
            this.verifyFilesBtn.Text = "Verify Files";
            this.verifyFilesBtn.UseVisualStyleBackColor = true;
            this.verifyFilesBtn.Click += new System.EventHandler(this.verifyFilesBtn_Click);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 87);
            this.Controls.Add(this.verifyFilesBtn);
            this.Controls.Add(this.scanFilesButton);
            this.Controls.Add(this.fileProgressBar);
            this.Controls.Add(this.selectFileButton);
            this.Name = "Form";
            this.Text = "StarrForensics";

        }

        #endregion

        private System.Windows.Forms.Button selectFileButton;
        private System.Windows.Forms.ProgressBar fileProgressBar;
        private System.Windows.Forms.Button scanFilesButton;
        private System.Windows.Forms.Button verifyFilesBtn;
    }
}

