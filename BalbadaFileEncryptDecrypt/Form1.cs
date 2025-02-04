using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BalbadaFileEncryptDecrypt
{
    public partial class Form1 : Form
    {
        // api key, for more secure we can add it to .env or windows manager, but for demo we hardcode it here
        private static string SecretKey = "x-balbada-47GjX9tLzQk82mYF6Cdw5P"; 

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string filePath = openFileDialog1.FileName;
            bool isEncrypted = EncryptFile(filePath);
            if (isEncrypted)
            {
               MessageBox.Show($"File encrypted: {filePath}.enc", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            string filePath = openFileDialog2.FileName;
            bool isDecrypted = DecryptFile(filePath);
            if (isDecrypted)
            {
               MessageBox.Show($"File decrypted: {filePath.Replace(".enc", "")}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        // encrypt uses AES
        private bool EncryptFile(string filePath)
        {
            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(SecretKey);
                byte[] iv = new byte[16];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(iv);
                }

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (FileStream fs = new FileStream(filePath + ".enc", FileMode.Create))
                    {
                        fs.Write(iv, 0, iv.Length); 

                        using (CryptoStream cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        using (FileStream fsInput = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            fsInput.CopyTo(cs);
                        }
                    }
                }

                // delete the file after encrypting
                File.Delete(filePath);
                return true;
            }
            catch(Exception err)
            {
                Console.WriteLine(err.ToString());
                MessageBox.Show("Something went wrong!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            
        }

        // decrypt, encryptApi == decryptApi
        private bool DecryptFile(string filePath)
        {
            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(SecretKey);

                using (FileStream fsInput = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] iv = new byte[16];
                    fsInput.Read(iv, 0, iv.Length);

                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = keyBytes;
                        aes.IV = iv;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;

                        using (CryptoStream cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))

                        // revert to non .enc or orig file ext
                        using (FileStream fsOutput = new FileStream(filePath.Replace(".enc", ""), FileMode.Create))
                        {
                            cs.CopyTo(fsOutput);
                        }
                    }
                }

                // delete the enc file after decrypting
                File.Delete(filePath);
                return true;

            }
            catch(Exception err)
            {
                Console.WriteLine(err.ToString());
                MessageBox.Show("Invalid API key!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

        }
    }
}