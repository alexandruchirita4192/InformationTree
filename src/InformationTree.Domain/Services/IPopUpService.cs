using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface IPopUpService
    {
        void ShowMessage(string text, string caption = null);

        void ShowInfo(string text, string caption = null);

        void ShowWarning(string text, string caption = null);

        void ShowError(string text, string caption = null);

        /// <summary>
        /// Shows a pop-up asking for confirmation
        /// </summary>
        /// <returns><see cref="PopUpResult.Confirm"/> or <see cref="PopUpResult.NotConfirm"/></returns>
        PopUpResult Confirm(string text, string caption = null);

        /// <summary>
        /// Shows a pop-up asking a question
        /// </summary>
        /// <returns><see cref="PopUpResult.Yes"/> or <see cref="PopUpResult.No"/></returns>
        PopUpResult ShowQuestion(string text, string caption = null, DefaultPopUpButton defaultButton = DefaultPopUpButton.Yes);

        /// <summary>
        /// Shows a pop-up asking a question that can also be canceled
        /// </summary>
        /// <returns><see cref="PopUpResult.Yes"/>, <see cref="PopUpResult.No"/> or <see cref="PopUpResult.Cancel"/></returns>
        PopUpResult ShowCancelableQuestion(string text, string caption = null, DefaultPopUpButton defaultButton = DefaultPopUpButton.Yes);

        string GetPrivateKeyFile();

        string GetPublicKeyFile();

        string GetXmlDataFile(string fileName, bool? fileNameExists = null);

        string GetImageFile();

        string GetRtfFile();
        
        string SaveRtfFile();
    }
}