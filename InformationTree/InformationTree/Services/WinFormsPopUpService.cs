using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class WinFormsPopUpService : IPopUpService
    {
        private readonly ISoundProvider _soundProvider;

        public WinFormsPopUpService(ISoundProvider soundProvider)
        {
            _soundProvider = soundProvider;
        }

        public void ShowMessage(string text, string caption = null)
        {
            if (string.IsNullOrWhiteSpace(caption))
            {
                MessageBox.Show(text);
            }
            else
            {
                MessageBox.Show(text, caption);
            }
        }

        public PopUpResult Confirm(string text, string caption = null)
        {
            _soundProvider.PlaySystemSound(4);

            var result = MessageBox.Show(text, caption ?? "Confirm", MessageBoxButtons.YesNo);

            return result == DialogResult.Yes ? PopUpResult.Confirm : PopUpResult.NotConfirm;
        }

        public bool ShowQuestion(string text, string caption = null, bool defaultButton = true)
        {
            var messsageBoxDefaultButton = defaultButton ? MessageBoxDefaultButton.Button1 : MessageBoxDefaultButton.Button2;
            var ret = MessageBox.Show(
                text,
                caption ?? "Question",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                messsageBoxDefaultButton);
            return ret == DialogResult.Yes;
        }

        public void ShowError(string text, string caption = null)
        {
            MessageBox.Show(text, caption ?? "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowInfo(string text, string caption = null)
        {
            MessageBox.Show(text, caption ?? "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowWarning(string text, string caption = null)
        {
            MessageBox.Show(text, caption ?? "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public string GetPrivateKeyFile()
        {
            return GetKeyFile("Private");
        }

        public string GetPublicKeyFile()
        {
            return GetKeyFile("Public");
        }

        private string GetKeyFile(string keyType)
        {
            var lowerFileType = keyType.ToLower();
            var dlg = new OpenFileDialog
            {
                Title = $"Open {lowerFileType} key file",
                Filter = $"{keyType} Key Files|*.asc;*.skr;*.{lowerFileType}|All files (*.*)|*.*",
                InitialDirectory = Application.StartupPath
            };

            return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
        }
    }
}