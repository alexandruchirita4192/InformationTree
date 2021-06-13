using FormsGame.PgpEncryption;
using FormsGame.TextProcessing;
using FormsGame.Tree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormsGame.Forms
{
    public partial class PopUpEditForm : Form
    {
        #region constants
        private const int WM_COPY = 0x301;
        private const int WM_CUT = 0x300;
        private const int WM_PASTE = 0x302;
        private const int WM_CLEAR = 0x303;
        private const int WM_UNDO = 0x304;
        private const int EM_UNDO = 0xC7;
        private const int EM_CANUNDO = 0xC6;
        #endregion

        #region Properties
        private string data;
        public string Data
        {
            get
            {
                if (data != null)
                    return data;
                if(tbData.TextLength == 0)
                    return string.Empty;
                return tbData.Rtf;
            }
        }

        bool DataIsPgpEncrypted
        {
            get
            {
                return tbData.Text.Trim().StartsWith("-----BEGIN PGP");
            }
        }


        public bool FromFile;

        public string PgpPassword;
        public string PgpKeyFile;
        public string PgpKeyText;


        CanvasPopUpForm CanvasForm;
        private static Stopwatch timer = new Stopwatch();

        #endregion

        #region Constructor

        public PopUpEditForm()
        {
            InitializeComponent();

            this.FormClosing += tbExitPopUpAndSave_Click;
            
            if(tbData != null && tbData.TextBox != null)
                tbData.TextBox.AllowDrop = true;

            this.tbData.TextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PopUpEditForm_KeyUp);
        }

        public PopUpEditForm(string title, string data)
            : this()
        {
            this.Text += ": " + title;

            try
            {
                tbData.Rtf = data;
            }
            catch (ArgumentException)
            {
                tbData.Text = data;
            }

            var richTextBoxForeColor = tbData.ForeColor;
            if (richTextBoxForeColor == TreeNodeHelper.DefaultBackGroundColor)
                tbData.ForeColor = TreeNodeHelper.DefaultForeGroundColor;

            var richTextBoxBackColor = tbData.BackColor;
            if (richTextBoxBackColor != TreeNodeHelper.DefaultBackGroundColor)
                tbData.BackColor = TreeNodeHelper.DefaultBackGroundColor;
            
            if(tbData != null && tbData.TextBox != null && tbData.TextBox.Text != null)
            {
                for (int i = 0; i < tbData.TextBox.Text.Length; i++)
                {
                    tbData.TextBox.Select(i, 1);
                    if (tbData.TextBox.SelectionColor == TreeNodeHelper.DefaultBackGroundColor)
                        tbData.TextBox.SelectionColor = TreeNodeHelper.DefaultForeGroundColor;

                    if (tbData.TextBox.SelectionBackColor != TreeNodeHelper.DefaultBackGroundColor)
                        tbData.TextBox.SelectionBackColor = TreeNodeHelper.DefaultBackGroundColor;
                }
            }
        }

        #endregion Constructor

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.V:
                case Keys.Shift | Keys.Insert:
                    if(Clipboard.ContainsImage())
                    {
                        var image = Clipboard.GetImage();
                        var maxWidth = tbData.Width - 10;
                        var maxHeight = tbData.Height - 10;

                        var newWidth = maxWidth;
                        var newHeight = image.Height * newWidth / image.Width;

                        if (newHeight > maxHeight)
                        {
                            // Resize with height instead  
                            newWidth = image.Width * maxHeight / image.Height;
                            newHeight = maxHeight;
                        }

                        if(newWidth != image.Width || newHeight != image.Height)
                        {
                            image = image.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);
                            Clipboard.SetImage(image);
                        }
                    }

                    return base.ProcessCmdKey(ref msg, keyData);
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void tbExitPopUpAndSave_Click(object sender, EventArgs e)
        {
            if(object.ReferenceEquals(sender, tbExitPopUpAndSave))
                this.Close();
        }

        private void tbData_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void tbData_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.B | Keys.Control))
                tbData.ToggleBold();
            //else if (e.KeyData == (Keys.I | Keys.Control))
            //    tbData.ToggleItalic();
            else if (e.KeyData == (Keys.U | Keys.Control))
                tbData.ToggleUnderline();
            else if (e.KeyData == (Keys.S | Keys.Control))
                tbData.ToggleStrikeOut();
            //else if (e.KeyData == (Keys.Left | Keys.Control))
            //    tbData.SetAlign(HorizontalAlignment.Left);
            //else if (e.KeyData == (Keys.Up | Keys.Control))
            //    tbData.SetAlign(HorizontalAlignment.Center);
            //else if (e.KeyData == (Keys.Right | Keys.Control))
            //    tbData.SetAlign(HorizontalAlignment.Right);
            else if (e.KeyData == (Keys.O | Keys.Control))
                tbData.OpenFile();
            else if (e.KeyData == (Keys.S | Keys.Control))
                tbData.SaveFile();
            else if (e.KeyData == (Keys.P | Keys.Control))
                tbData.InsertPicture();
        }

        private void btnPgpDecryptData_Click(object sender, EventArgs e)
        {
            if (cbKeepCrypt.Checked)
                data = Data;
            else
                data = null;

            if (DataIsPgpEncrypted || (MessageBox.Show("Data seems to be decrypted. Try to decrypt anyway?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                PgpDecrypt form = new PgpDecrypt(cbFromFile.Checked);
                if (!form.IsDisposed)
                { 
                    Program.CenterForm(form, this);

                    form.FormClosed += PgpDecryptForm_FormClosed;
                    form.ShowDialog();
                }
            }
        }

        private void PgpDecryptForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var form = sender as PgpDecrypt;
            if (form != null)
            {
                PgpPassword = form.PgpPassword;

                //FromFile = form.DecryptFromFile;
                PgpKeyFile = form.PgpPrivateKeyFile;
                PgpKeyText = form.PgpPrivateKeyText;

                var result = string.Empty;
                if(FromFile)
                {
                    result = PGPEncryptDecrypt.GetDecryptedStringFromFile(tbData.Text, PgpKeyFile, PgpPassword);
                    lblEncryption.Text = "Decrypted with key: " + PgpKeyFile;
                }
                else
                {
                    result = PGPEncryptDecrypt.GetDecryptedStringFromString(tbData.Text, PgpKeyText, PgpPassword);
                    lblEncryption.Text = "Decrypted with node key";
                }

                tbData.Rtf = result;
            }
        }

        private void btnPgpEncryptData_Click(object sender, EventArgs e)
        {
            FromFile = cbFromFile.Checked;

            var result = MessageBox.Show("Encrypt RTF? (else get only plaintext)", "Encrypt RTF?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            var decryptedData = string.Empty;

            if (result == DialogResult.Yes)
                decryptedData = tbData.Rtf;
            else
                decryptedData = tbData.Text;

            if (FromFile)
            {
                PgpKeyFile = PGPEncryptDecrypt.GetPublicKeyFile();

                using (var decryptedStream = PGPEncryptDecrypt.GenerateStreamFromString(decryptedData))
                {
                    using (var outputStream = new MemoryStream())
                    {
                        PGPEncryptDecrypt.EncryptFromFile(decryptedStream, outputStream, PgpKeyFile, true, true);
                        using (var reader = new StreamReader(outputStream))
                        {
                            var resultedText = reader.ReadToEnd();
                            if(tbData.Text != resultedText)
                            {
                                data = null;
                                tbData.Text = resultedText;
                                lblEncryption.Text = "Encrypted with key: " + PgpKeyFile;
                            }
                        }
                    }
                }
            }
            else
            {
                FindNodeWithPublicKey();

                var resultedText = RicherTextBox.Controls.RicherTextBox.StripRTF(PGPEncryptDecrypt.GetEncryptedStringFromString(tbData.Rtf, PgpKeyText, true, true));
                if (tbData.Text != resultedText)
                {
                    data = null;
                    tbData.Text = resultedText;
                    lblEncryption.Text = "Encrypted with node key";
                }
            }
        }

        private void FindNodeWithPublicKey()
        {
            var form = new SearchForm();

            Program.CenterForm(form, Program.MainForm);

            form.FormClosed += SearchForm_FormClosed;
            form.ShowDialog();
        }

        private void SearchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var form = sender as SearchForm;
            if (form != null)
            {
                var textToFind = form.TextToFind;

                if (Program.MainForm == null || Program.MainForm.TaskList == null || Program.MainForm.TaskList.Nodes == null)
                    return;

                var node = TreeNodeHelper.GetFirstNode(Program.MainForm.TaskList.Nodes, textToFind);
                if (node != null)
                {
                    var nodeData = node.Tag as TreeNodeData;
                    if (nodeData != null)
                    {
                        PgpKeyText = RicherTextBox.Controls.RicherTextBox.StripRTF(nodeData.Data);

                        MessageBox.Show("Public key taken from data of node " + node.Text, "Node " + node.Text + " used", MessageBoxButtons.OK);
                    }
                }

                //if (string.IsNullOrEmpty(PgpPrivateKeyText) &&
                //    MessageBox.Show("You did not select a valid private key node. Try to select again?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                //    FindNodeWithPublicKey();
            }
        }

        private void PopUpEditForm_DoubleClick(object sender, EventArgs e)
        {
            if (this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.Sizable)
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            else
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        }

        private void PopUpEditForm_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not supported");
        }

        private void btnShowGraphics_Click(object sender, EventArgs e)
        {
            var text = tbData.TextBox.SelectedText.Length > 0 ? tbData.TextBox.SelectedText : tbData.TextBox.Text;
            if (text.Length <= 0)
                return;

            if (CanvasForm == null || CanvasForm.IsDisposed)
                CanvasForm = new CanvasPopUpForm();
            CanvasForm.RunTimer.Stop();
            CanvasForm.GraphicsFile.Clean();
            CanvasForm.GraphicsFile.ParseLines(text);
            CanvasForm.RunTimer.Start();
            CanvasForm.Show();
        }
    }
}
