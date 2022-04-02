using InformationTree.Domain.Entities;

namespace InformationTree.Domain.Services
{
    public interface IPopUpConfirmation
    {
        PopUpResult Confirm(string message, string title);
    }
}