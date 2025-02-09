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
        // Playfair cipher key
        private static string SecretKey = "BALBADAFILE";

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

        private bool EncryptFile(string filePath)
        {
            try
            {
                string plaintext = File.ReadAllText(filePath);
                string encryptedText = EncryptPlayfair(plaintext, SecretKey);
                File.WriteAllText(filePath + ".enc", encryptedText);
                File.Delete(filePath);
                return true;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                MessageBox.Show("Encryption failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool DecryptFile(string filePath)
        {
            try
            {
                string encryptedText = File.ReadAllText(filePath);
                string decryptedText = DecryptPlayfair(encryptedText, SecretKey);
                File.WriteAllText(filePath.Replace(".enc", ""), decryptedText);
                File.Delete(filePath);
                return true;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                MessageBox.Show("Decryption failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private char[,] GeneratePlayfairMatrix(string key)
        {
            key = new string(key.ToUpper().Distinct().ToArray()).Replace("J", "I");
            string alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ";
            string fullKey = key + new string(alphabet.Where(c => !key.Contains(c)).ToArray());

            char[,] matrix = new char[5, 5];
            int index = 0;
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    matrix[row, col] = fullKey[index++];
                }
            }
            return matrix;
        }

        private (int, int) FindPosition(char[,] matrix, char letter)
        {
            if (letter == 'J') letter = 'I';
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    if (matrix[row, col] == letter)
                        return (row, col);
                }
            }
            return (-1, -1);
        }

        private string PreparePlayfairInput(string text)
        {
            text = new string(text.ToUpper().Where(char.IsLetter).ToArray()).Replace("J", "I");
            StringBuilder formattedText = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                formattedText.Append(text[i]);
                if (i + 1 < text.Length && text[i] == text[i + 1])
                {
                    formattedText.Append('X');
                }
            }

            if (formattedText.Length % 2 != 0)
            {
                formattedText.Append('X');
            }

            return formattedText.ToString();
        }

        private string ProcessPlayfair(string text, bool encrypt, string key)
        {
            char[,] matrix = GeneratePlayfairMatrix(key);
            text = PreparePlayfairInput(text);

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < text.Length; i += 2)
            {
                char a = text[i], b = text[i + 1];
                var (row1, col1) = FindPosition(matrix, a);
                var (row2, col2) = FindPosition(matrix, b);

                if (row1 == row2)
                {
                    col1 = (col1 + (encrypt ? 1 : 4)) % 5;
                    col2 = (col2 + (encrypt ? 1 : 4)) % 5;
                }
                else if (col1 == col2)
                {
                    row1 = (row1 + (encrypt ? 1 : 4)) % 5;
                    row2 = (row2 + (encrypt ? 1 : 4)) % 5;
                }
                else
                {
                    (col1, col2) = (col2, col1);
                }

                result.Append(matrix[row1, col1]).Append(matrix[row2, col2]);
            }
            return result.ToString();
        }

        private string EncryptPlayfair(string text, string key) => ProcessPlayfair(text, true, key);
        private string DecryptPlayfair(string text, string key) => ProcessPlayfair(text, false, key);
    }
}