using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface IPopUpService
    {
        void ShowMessage(string text, string caption = null);

        void ShowInfo(string text, string caption = null);

        bool ShowQuestion(string text, string caption = null, bool defaultButton = true);

        void ShowWarning(string text, string caption = null);

        void ShowError(string text, string caption = null);

        PopUpResult Confirm(string text, string caption = null);
    }
}