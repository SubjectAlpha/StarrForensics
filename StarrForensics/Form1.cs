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
                if(FBD.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(FBD.SelectedPath))
                {
                    SelectedPath = FBD.SelectedPath;
                }
            }
        }

        private void scanFilesButton_Click(object sender, EventArgs e)
        {
            int CurrentCount = 0;
            int FileCount = 0;
            string CurrentFileName = "";
            string SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string HashFile = SaveDirectory + @$"\hashData{DateTime.Now.ToString("MMddyyHHmmss")}.json";
            List<HashFormat> HashData = new List<HashFormat>();

            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            worker.RunWorkerAsync();

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

                    List<FileInfo> files = Directory.GetFiles("*.*", SearchOption.AllDirectories).ToList();
                    FileCount = files.Count();

                    using(SHA512 hash = SHA512.Create())
                    {
                        foreach(FileInfo fInfo in files)
                        {
                            try
                            {
                                using(FileStream fileStream = fInfo.OpenRead())
                                {
                                    fileStream.Position = 0;
                                    CurrentFileName = fileStream.Name;
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

                            CurrentCount++;
                            worker.ReportProgress(CurrentCount);
                        }
                    }

                    string jsonToWrite = JsonConvert.SerializeObject(HashData.ToArray());

                    File.WriteAllText(HashFile, jsonToWrite);
                }
                else
                {
                    MessageBox.Show("Directory not found");
                }
            });

            worker.ProgressChanged += new ProgressChangedEventHandler(delegate (object o, ProgressChangedEventArgs args)
            {
                currentFileBox.Text = CurrentFileName;
                fileProgressBar.Value = (CurrentCount / FileCount) * 100;
            });


            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate (object o, RunWorkerCompletedEventArgs args)
            {
                currentFileBox.Text = "Complete";
                SystemSounds.Beep.Play();
                worker.Dispose();
            });
        }

        private void verifyFilesBtn_Click(object sender, EventArgs e)
        {
            bool FoundMissedHashes = false;

            string VerificationFileName = "";
            string CurrentFileName = "";

            int CurrentCount = 0;
            int FileCount = 0;

            using (OpenFileDialog OFD = new OpenFileDialog())
            {
                OFD.Filter = "Json files (*.json)|*.json";

                if (OFD.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(OFD.FileName))
                {
                    VerificationFileName = OFD.FileName;
                }
            }

            if(VerificationFileName != "")
            {
                BackgroundWorker bw = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true,
                };

                bw.RunWorkerAsync();

                bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                {
                    List<HashFormat> HashesToVerify = new List<HashFormat>();
                    List<MissedHash> MissedHashes = new List<MissedHash>();

                    FileInfo VerificationFile = new FileInfo(VerificationFileName);

                    DirectoryInfo PreviouslySelectedDir = new DirectoryInfo(SelectedPath);
                    List<FileInfo> files = PreviouslySelectedDir.GetFiles("*.*", SearchOption.AllDirectories).ToList();

                    FileCount = files.Count();

                    using (StreamReader r = new StreamReader(VerificationFile.FullName))
                    {
                        string data = r.ReadToEnd();
                        HashesToVerify.AddRange(JsonConvert.DeserializeObject<List<HashFormat>>(data));
                    }

                    using (SHA512 hash = SHA512.Create())
                    {
                        foreach (FileInfo file in files)
                        {
                            try
                            {
                                using (FileStream fileStream = file.OpenRead())
                                {
                                    CurrentFileName = fileStream.Name;
                                    byte[] HashValue = hash.ComputeHash(fileStream);
                                    StringBuilder SB = new StringBuilder(128);
                                    foreach (byte b in HashValue)
                                    {
                                        SB.Append(b.ToString("X2"));
                                    }

                                    if (HashesToVerify.Where(x => x.FileName == fileStream.Name).Count() > 0)
                                    {
                                        HashFormat Hash = HashesToVerify.Single(x => x.FileName == fileStream.Name);

                                        if (SB.ToString() != Hash.SHA512Hash)
                                        {
                                            MissedHashes.Add(new MissedHash
                                            {
                                                FoundFileName = fileStream.Name,
                                                ExpectedFileName = Hash.FileName,
                                                FoundHash = SB.ToString(),
                                                ExpectedHash = Hash.SHA512Hash
                                            });

                                            FoundMissedHashes = true;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Unexpected file found, please generate a new hash file for this directory");
                                        break;
                                    }
                                }
                            }
                            catch (IOException e)
                            {
                                MessageBox.Show($"An error occurred: { e.Message }");
                            }
                            catch (UnauthorizedAccessException e)
                            {
                                MessageBox.Show($"Access Error: { e.Message }");
                            }

                            CurrentCount++;
                            bw.ReportProgress(CurrentCount);
                        }
                    }
                });

                bw.ProgressChanged += new ProgressChangedEventHandler(delegate (object o, ProgressChangedEventArgs args)
                {
                    currentFileBox.Text = CurrentFileName;
                    fileProgressBar.Value = (CurrentCount / FileCount) * 100;
                });

                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate (object o, RunWorkerCompletedEventArgs args)
                {
                    currentFileBox.Text = "Complete";
                    SystemSounds.Beep.Play();
                    if (FoundMissedHashes)
                    {
                        MessageBox.Show("Found differences in selected directory");
                    }
                    bw.Dispose();
                });
            }
            else
            {
                MessageBox.Show("You must select a file first!");
            }
            
        }
    }
}
