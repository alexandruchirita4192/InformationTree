using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Forms;
using InformationTree.Render.WinForms.Extensions;
using InformationTree.Render.WinForms.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class PgpEncryptDecryptDataHandler : IRequestHandler<PgpEncryptDecryptDataRequest, BaseResponse>
    {
        public PgpEncryptDecryptDataHandler(
            IPopUpService popUpService,
            IPGPEncryptionAndSigningProvider encryptionAndSigningProvider,
            IConfigurationReader configurationReader)
        {
            _popUpService = popUpService;
            _encryptionAndSigningProvider = encryptionAndSigningProvider;
            _configurationReader = configurationReader;
            _configuration = _configurationReader.GetConfiguration();
        }

        #region Services

        private readonly IPopUpService _popUpService;
        private readonly IPGPEncryptionAndSigningProvider _encryptionAndSigningProvider;
        private readonly IConfigurationReader _configurationReader;

        #endregion Services

        private readonly Configuration _configuration;

        #region PGP

        private string _pgpPublicKeyFile;
        private string _pgpPublicKeyText;
        private string _pgpPrivateKeyText;
        private string _pgpPassword;
        private string _pgpPrivateKeyFile;

        #endregion PGP

        #region Request

        private PgpEncryptDecryptDataRequest _request;
        private Form _formToCenterTo;

        #endregion Request

        private PgpEncryptDecryptDataResponse _response;

        public Task<BaseResponse> Handle(PgpEncryptDecryptDataRequest request, CancellationToken cancellationToken)
        {
            if (request.FormToCenterTo is not Form formCenterTo)
                return Task.FromResult<BaseResponse>(null);

            _request = request;
            _formToCenterTo = formCenterTo;
            _response = new PgpEncryptDecryptDataResponse();

            return Task.FromResult(InternalHandle());
        }

        private BaseResponse InternalHandle()
        {
            if (_request.ActionType == PgpActionType.Encrypt)
            {
                var result = _popUpService.ShowQuestion("Do you want to encrypt as RTF? (Otherwise it would be text only.)", "Encrypt as RTF?", DefaultPopUpButton.No);
                var decryptedData = result == PopUpResult.Yes ? _request.InputDataRtf : _request.InputDataText;

                if (decryptedData.IsEmpty())
                {
                    _popUpService.ShowError("No data to encrypt.");
                    return null;
                }

                if (_request.FromFile)
                {
                    _pgpPublicKeyFile = _popUpService.GetPublicKeyFile();

                    if (_configuration.TreeFeatures.EnableEncryptionSigning)
                    {
                        _pgpPublicKeyText = File.ReadAllText(_pgpPublicKeyFile);

                        GetPrivateKeyWithPassword("Get private key required for signing encrypted data");

                        var resultedText = _encryptionAndSigningProvider.EncryptAndSignString(_request.InputDataRtf, _pgpPublicKeyText, _pgpPrivateKeyText, _pgpPassword, true);
                        resultedText = RicherTextBox.Controls.RicherTextBox.StripRTF(resultedText);

                        if (_request.InputDataText != resultedText)
                        {
                            _response.ResultText = resultedText;
                            _response.EncryptionInfo = $"Encrypted with key: {_pgpPublicKeyFile}";
                        }
                        return _response;
                    }
                    else
                    {
                        using var decryptedStream = decryptedData.ToStream();
                        using var outputStream = new MemoryStream();

                        _encryptionAndSigningProvider.EncryptFromFile(decryptedStream, outputStream, _pgpPublicKeyFile, true, true);

                        using var reader = new StreamReader(outputStream);

                        var resultedText = reader.ReadToEnd();
                        if (_request.InputDataText != resultedText)
                        {
                            _response.ResultText = resultedText;
                            _response.EncryptionInfo = $"Encrypted with key: {_pgpPublicKeyFile}";
                        }
                        return _response;
                    }
                }
                else
                {
                    FindNodeWithPublicKey();

                    string resultedText;
                    if (_configuration.TreeFeatures.EnableEncryptionSigning)
                    {
                        GetPrivateKeyWithPassword("Get private key required for signing encrypted data");

                        resultedText = _encryptionAndSigningProvider.EncryptAndSignString(_request.InputDataRtf, _pgpPublicKeyText, _pgpPrivateKeyText, _pgpPassword, true);
                    }
                    else
                    {
                        resultedText = _encryptionAndSigningProvider.GetEncryptedStringFromString(_request.InputDataRtf, _pgpPublicKeyText, true, true);
                    }
                    resultedText = RicherTextBox.Controls.RicherTextBox.StripRTF(resultedText);

                    if (_request.InputDataText != resultedText)
                    {
                        _response.ResultText = resultedText;
                        _response.EncryptionInfo = "Encrypted with node key";
                    }
                    return _response;
                }
            }
            else if (_request.ActionType == PgpActionType.Decrypt)
            {
                if (_request.InputDataText.IsEmpty())
                {
                    _popUpService.ShowError("No data to decrypt.");
                    return null;
                }

                if (_request.DataIsPgpEncrypted
                || _popUpService.ShowQuestion("Data seems to be decrypted. Try to decrypt anyway?") == PopUpResult.Yes)
                {
                    GetPrivateKeyWithPassword();
                }
                return _response;
            }

            return null;
        }

        private void FindNodeWithPublicKey()
        {
            var form = new SearchForm(_popUpService);

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
                    _pgpPublicKeyText = RicherTextBox.Controls.RicherTextBox.StripRTF(nodeData.Data);

                    _popUpService.ShowMessage($"Public key taken from data of node {nodeData.Text}", $"Node {nodeData.Text} used");
                }

                if (_pgpPublicKeyText.IsEmpty() && _popUpService.ShowQuestion("You did not select a valid public key node. Try to select again?") == PopUpResult.Yes)
                    FindNodeWithPublicKey();
            }
        }

        private void GetPrivateKeyWithPassword(string titleOverride = null)
        {
            var form = new PgpDecrypt(_popUpService, _encryptionAndSigningProvider, _request.FromFile);

            if (titleOverride.IsNotEmpty())
                form.Text = titleOverride;

            WinFormsApplication.CenterForm(form, _formToCenterTo);

            form.FormClosed += PgpDecryptForm_FormClosed;
            form.ShowDialog();
        }

        private void PgpDecryptForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is PgpDecrypt form)
            {
                _pgpPassword = form.PgpPassword;
                _pgpPrivateKeyFile = form.PgpPrivateKeyFile;
                _pgpPrivateKeyText = form.PgpPrivateKeyText;

                string result;
                if (_request.FromFile)
                {
                    result = _encryptionAndSigningProvider.GetDecryptedStringFromFile(_request.InputDataText, _pgpPrivateKeyFile, _pgpPassword);

                    _response.EncryptionInfo = $"Decrypted with key: {_pgpPrivateKeyFile}";
                }
                else
                {
                    result = _encryptionAndSigningProvider.GetDecryptedStringFromString(_request.InputDataText, _pgpPrivateKeyText, _pgpPassword);

                    _response.EncryptionInfo = "Decrypted with node key";
                }

                _response.ResultRtf = result;
            }
        }
    }
}