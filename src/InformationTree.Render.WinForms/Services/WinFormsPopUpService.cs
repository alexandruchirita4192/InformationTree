using System.IO;
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

        public void ShowInfo(string text, string caption = null)
        {
            MessageBox.Show(text, caption ?? "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowWarning(string text, string caption = null)
        {
            MessageBox.Show(text, caption ?? "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ShowError(string text, string caption = null)
        {
            MessageBox.Show(text, caption ?? "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public PopUpResult Confirm(string text, string caption = null)
        {
            _soundProvider.PlaySystemSound(4);

            var result = MessageBox.Show(text, caption ?? "Confirm", MessageBoxButtons.YesNo);

            return result == DialogResult.Yes ? PopUpResult.Confirm : PopUpResult.NotConfirm;
        }

        public PopUpResult ShowQuestion(string text, string caption = null, DefaultPopUpButton defaultButton = DefaultPopUpButton.Yes)
        {
            MessageBoxDefaultButton messsageBoxDefaultButton;

            switch (defaultButton)
            {
                default:
                case DefaultPopUpButton.Yes:
                    messsageBoxDefaultButton = MessageBoxDefaultButton.Button1;
                    break;
                case DefaultPopUpButton.No:
                    messsageBoxDefaultButton = MessageBoxDefaultButton.Button2;
                    break;
            }
            
            var dialogResult = MessageBox.Show(
                text,
                caption ?? "Question",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                messsageBoxDefaultButton);

            PopUpResult result;

            switch (dialogResult)
            {
                case DialogResult.Yes:
                    result = PopUpResult.Yes;
                    break;
                default:
                case DialogResult.No:
                    result = PopUpResult.No;
                    break;
            }

            return result;
        }

        public PopUpResult ShowCancelableQuestion(string text, string caption = null, DefaultPopUpButton defaultButton = DefaultPopUpButton.Yes)
        {
            MessageBoxDefaultButton messsageBoxDefaultButton;
            
            switch (defaultButton)
            {
                default:
                case DefaultPopUpButton.Yes:
                    messsageBoxDefaultButton = MessageBoxDefaultButton.Button1;
                    break;
                case DefaultPopUpButton.No:
                    messsageBoxDefaultButton = MessageBoxDefaultButton.Button2;
                    break;
                case DefaultPopUpButton.Cancel:
                    messsageBoxDefaultButton = MessageBoxDefaultButton.Button3;
                    break;
            }
            
            var dialogResult = MessageBox.Show(
                text,
                caption ?? "Question",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                messsageBoxDefaultButton);

            PopUpResult result;
            
            switch(dialogResult)
            {
                case DialogResult.Yes:
                    result = PopUpResult.Yes;
                    break;
                case DialogResult.No:
                    result = PopUpResult.No;
                    break;
                default:
                case DialogResult.Cancel:
                    result = PopUpResult.Cancel;
                    break;
            }
            
            return result;
        }

        public string GetPrivateKeyFile()
        {
            return GetKeyFile("Private");
        }

        public string GetPublicKeyFile()
        {
            return GetKeyFile("Public");
        }

        public string GetXmlDataFile(string fileName, bool? fileNameExists = null)
        {
            var title = "Open XML Document";
            var filter = "XML Files (*.xml)|*.xml*|All files (*.*)|*.*";

            return GetFile(title, filter, fileName, fileNameExists);
        }

        public string GetImageFile()
        {
            var title = "Open picture";
            var filter = "Picture|*.png;*.jpg;*.jpeg;*.bmp;*.gif;PNG Files|*.png|JPEG Files|*.jpg;*.jpeg|Bitmap Files|*.bmp|GIF Files|*.gif|All files|*.*";
            var defaultExtension = "jpg";

            return GetFile(title, filter, null, false, defaultExtension);
        }

        public string GetRtfFile()
        {
            var title = "Open RTF Document";
            var filter = "Rich Text Format|*.rtf";
            var defaultExtension = "rtf";
            
            return GetFile(title, filter, null, false, defaultExtension);
        }
        
        public string SaveRtfFile()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Rich Text Format|*.rtf",
                FilterIndex = 0,
                OverwritePrompt = true
            };

            var selectedFile = dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : string.Empty;
            dialog.Dispose();

            return selectedFile;
        }

        private string GetKeyFile(string keyType)
        {
            var lowerFileType = keyType.ToLower();
            var title = $"Open {lowerFileType} key file";
            var filter = $"{keyType} Key Files|*.asc;*.skr;*.{lowerFileType}|All files (*.*)|*.*";

            return GetFile(title, filter, null, false);
        }

        private string GetFile(string title, string filter, string selectedFileName, bool? selectedFileNameExists = null, string defaultExtension = null)
        {
            if (string.IsNullOrWhiteSpace(selectedFileName))
                selectedFileNameExists = false;
            else if (selectedFileNameExists == null)
                selectedFileNameExists = File.Exists(selectedFileName);

            var dialog = new OpenFileDialog
            {
                Title = title,
                Filter = filter
            };

            if (selectedFileNameExists ?? false)
                dialog.FileName = selectedFileName;

            if (!string.IsNullOrWhiteSpace(defaultExtension))
                dialog.DefaultExt = defaultExtension;

            var result = dialog.ShowDialog();

            var returnedFileName = result == DialogResult.OK ? dialog.FileName : string.Empty;

            dialog.Dispose();

            return returnedFileName;
        }
    }
}