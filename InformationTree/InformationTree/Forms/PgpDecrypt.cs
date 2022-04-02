using System;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.PgpEncryption;
using InformationTree.Render.WinForms.Services;
using InformationTree.Tree;

namespace InformationTree.Forms
{
    public partial class PgpDecrypt : Form
    {
        #region Properties

        public string PgpPassword
        {
            get
            {
                return mtbPgpDecrypt.Text;
            }
        }

        public bool DecryptFromFile;

        public string PgpPrivateKeyFile;
        public string PgpPrivateKeyText;

        #endregion Properties

        public PgpDecrypt()
        {
            InitializeComponent();

            mtbPgpDecrypt.PasswordChar = '#';
        }

        public PgpDecrypt(bool fromFile)
            : this()
        {
            DecryptFromFile = fromFile;

            if (DecryptFromFile)
            {
                PgpPrivateKeyFile = PGPEncryptDecrypt.GetPrivateKeyFile();
                while (string.IsNullOrEmpty(PgpPrivateKeyFile) &&
                    MessageBox.Show("You did not select a private key file. Try to select again?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    PgpPrivateKeyFile = PGPEncryptDecrypt.GetPrivateKeyFile();

                if (string.IsNullOrEmpty(PgpPrivateKeyFile))
                {
                    this.Close();
                    return;
                }
            }
            else
            {
                FindNodeWithPrivateKey();
            }
        }

        private void FindNodeWithPrivateKey()
        {
            var form = new SearchForm();

            WinFormsApplication.CenterForm(form, WinFormsApplication.MainForm);

            form.FormClosed += SearchForm_FormClosed;
            form.ShowDialog();
        }

        private void SearchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var form = sender as SearchForm;
            if (form != null)
            {
                var textToFind = form.TextToFind;

                if (WinFormsApplication.MainForm == null || WinFormsApplication.MainForm.TaskList == null || WinFormsApplication.MainForm.TaskList.Nodes == null)
                    return;

                var node = TreeNodeHelper.GetFirstNode(WinFormsApplication.MainForm.TaskList.Nodes, textToFind);
                if (node != null)
                {
                    var nodeData = node.Tag as TreeNodeData;
                    if (nodeData != null)
                    {
                        PgpPrivateKeyText = RicherTextBox.Controls.RicherTextBox.StripRTF(nodeData.Data);

                        MessageBox.Show("Private key taken from data of node " + node.Text, "Node " + node.Text + " used", MessageBoxButtons.OK);
                    }
                }

                //if (string.IsNullOrEmpty(PgpPrivateKeyText) &&
                //    MessageBox.Show("You did not select a valid private key node. Try to select again?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                //    FindNodeWithPrivateKey();
            }
        }

        private void mtbPgpDecrypt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var password = mtbPgpDecrypt.Text;

                //if (password.Length == 0)
                //    return;

                if (object.ReferenceEquals(sender, mtbPgpDecrypt))
                {
                    if (DecryptFromFile)
                    {
                        if (PGPEncryptDecrypt.ExistsPassword(PgpPrivateKeyFile, password.ToCharArray()))
                            this.Close();
                        //else
                        //    MessageBox.Show("Password is not valid with the selected PGP file", "Invalid password", MessageBoxButtons.OK);
                    }
                    else
                    {
                        if (PGPEncryptDecrypt.ExistsPasswordFromString(PgpPrivateKeyText, password.ToCharArray()))
                            this.Close();
                        //else
                        //    MessageBox.Show("Password is not valid with the selected PGP key", "Invalid password", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void lblDecryptingPassword_Click(object sender, EventArgs e)
        {
            PgpPrivateKeyFile = PGPEncryptDecrypt.GetPrivateKeyFile();
        }
    }
}