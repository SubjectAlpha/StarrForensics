/*
 * Jacob Starr
 * November 7, 2019
 * File hash generator
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace StarrForensics
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        private string SelectedPath = null;

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            using(FolderBrowserDialog FBD = new FolderBrowserDialog())
            {
                DialogResult result = FBD.ShowDialog();
                if(result == DialogResult.OK && !string.IsNullOrWhiteSpace(FBD.SelectedPath))
                {
                    SelectedPath = FBD.SelectedPath;
                }
            }
        }

        private void scanFilesButton_Click(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            int CurrentCount = 0;
            int FileCount = 0;
            string SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string HashFile = SaveDirectory + @$"\hashData{DateTime.Now.ToString("MMddyyHHmmss")}.json";
            List<HashFormat> HashData = new List<HashFormat>();

            worker.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
            {
                if (Directory.Exists(SelectedPath))
                {
                    DirectoryInfo Directory = new DirectoryInfo(SelectedPath);
                    DirectoryInfo SaveDirectoryInfo = new DirectoryInfo(SaveDirectory);

                    if (!File.Exists(HashFile)) {
                        FileStream newFile = File.Create(HashFile);
                        newFile.Close();
                    }

                    FileInfo[] files = Directory.GetFiles("*.*", SearchOption.AllDirectories);
                    FileCount = files.Count();

                    using(SHA512 hash = SHA512.Create())
                    {
                        foreach(FileInfo fInfo in files)
                        {
                            try
                            {
                                using(FileStream fileStream = fInfo.Open(FileMode.Open))
                                {
                                    fileStream.Position = 0;
                                    byte[] HashValue = hash.ComputeHash(fileStream);

                                    StringBuilder SB = new StringBuilder(128);
                                    foreach(byte b in HashValue)
                                    {
                                        SB.Append(b.ToString("X2"));
                                    }

                                    HashData.Add(new HashFormat
                                    {
                                        FileName = fileStream.Name,
                                        SHA512Hash = SB.ToString()
                                    });

                                    CurrentCount++;
                                    worker.ReportProgress(CurrentCount);
                                }
                            }
                            catch (IOException e)
                            {
                                MessageBox.Show($"An error occurred: { e.Message }");
                            }
                            catch(UnauthorizedAccessException e)
                            {
                                MessageBox.Show($"Access Error: { e.Message }");
                            }
                        }
                    }

                    string jsonToWrite = JsonConvert.SerializeObject(HashData.ToArray());

                    File.WriteAllText(HashFile, jsonToWrite);

                    SystemSounds.Beep.Play();
                }
                else
                {
                    MessageBox.Show("Directory not found");
                }
            });

            worker.ProgressChanged += new ProgressChangedEventHandler(delegate (object o, ProgressChangedEventArgs args)
            {
                fileProgressBar.Value = (CurrentCount / FileCount) * 100;
            });

            worker.RunWorkerAsync();
            worker.Dispose();
        }

        private void verifyFilesBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
