using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class WinFormsPopUpConfirmation : IPopUpConfirmation
    {
        private readonly ISoundProvider _soundProvider;

        public WinFormsPopUpConfirmation(ISoundProvider soundProvider)
        {
            _soundProvider = soundProvider;
        }
        
        public PopUpResult Confirm(string message, string title)
        {
            _soundProvider.PlaySystemSound(4);
            
            var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo);

            return result == DialogResult.Yes ? PopUpResult.Confirm : PopUpResult.NotConfirm;
        }
    }
}