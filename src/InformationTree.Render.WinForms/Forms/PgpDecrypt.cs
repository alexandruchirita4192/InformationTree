using System;
using System.IO;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Services;
using InformationTree.Render.WinForms.Extensions;
using InformationTree.Render.WinForms.Services;
using MediatR;

namespace InformationTree.Forms
{
    public partial class PgpDecrypt : Form
    {
        #region Fields

        private readonly IPopUpService _popUpService;
        private readonly IPGPEncryptionProvider _encryptionAndSigningProvider;
        private readonly IMediator _mediator;

        #endregion Fields

        #region Properties

        public string PgpPassword
        {
            get
            {
                return mtbPgpDecrypt.Text;
            }
        }

        public bool DecryptFromFile { get; set; }
        public string PgpPrivateKeyFile { get; private set; }
        public string PgpPrivateKeyText { get; private set; }

        #endregion Properties

        private PgpDecrypt()
        {
            InitializeComponent();

            mtbPgpDecrypt.PasswordChar = '#';
        }

        public PgpDecrypt(
            IPopUpService popUpService,
            IPGPEncryptionProvider encryptionAndSigningProvider,
            IMediator mediator,
            bool fromFile)
            : this()
        {
            _popUpService = popUpService;
            _encryptionAndSigningProvider = encryptionAndSigningProvider;
            _mediator = mediator;
            
            DecryptFromFile = fromFile;

            if (DecryptFromFile)
            {
                PgpPrivateKeyFile = _popUpService.GetPrivateKeyFile();
                while (PgpPrivateKeyFile.IsEmpty() &&
                    _popUpService.ShowQuestion("You did not select a private key file. Try to select again?") == PopUpResult.Yes)
                    PgpPrivateKeyFile = _popUpService.GetPrivateKeyFile();

                if (PgpPrivateKeyFile.IsNotEmpty())
                {
                    PgpPrivateKeyText = File.ReadAllText(PgpPrivateKeyFile);
                }
                else
                {
                    Close();
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
            var form = new SearchForm(_mediator);

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

                if (WinFormsApplication.MainForm?.TaskListRoot == null)
                    return;

                var nodeData = WinFormsApplication.MainForm.TaskListRoot.GetFirstNodeWith(textToFind);
                if (nodeData != null)
                {
                    PgpPrivateKeyText = nodeData.Data.StripRTF();

                    _popUpService.ShowMessage($"Private key taken from data of node {nodeData.Text}", $"Node {nodeData.Text} used");
                }

                if (PgpPrivateKeyText.IsEmpty() &&
                    _popUpService.ShowQuestion("You did not select a valid private key node. Try to select again?") == PopUpResult.Yes)
                    FindNodeWithPrivateKey();
            }
        }

        private void mtbPgpDecrypt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var password = mtbPgpDecrypt.Text;

                if (password.Length == 0)
                {
                    _popUpService.ShowWarning("You did not enter a password. Trying to decrypt without password.");
                }

                if (ReferenceEquals(sender, mtbPgpDecrypt))
                {
                    if (DecryptFromFile)
                    {
                        if (_encryptionAndSigningProvider.ExistsPassword(PgpPrivateKeyFile, password.ToCharArray()))
                            Close();
                        else
                            _popUpService.ShowMessage("Password is not valid for the selected PGP file", "Invalid password or PGP file");
                    }
                    else
                    {
                        if (_encryptionAndSigningProvider.ExistsPasswordFromString(PgpPrivateKeyText, password.ToCharArray()))
                            Close();
                        else
                            _popUpService.ShowMessage("Password is not valid for the selected PGP key", "Invalid password or PGP file");
                    }
                }
            }
        }

        private void lblDecryptingPassword_Click(object sender, EventArgs e)
        {
            PgpPrivateKeyFile = _popUpService.GetPrivateKeyFile();
        }
    }
}