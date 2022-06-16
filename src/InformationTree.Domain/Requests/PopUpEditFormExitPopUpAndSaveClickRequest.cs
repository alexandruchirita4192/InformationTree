using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class PopUpEditFormExitPopUpAndSaveClickRequest : BaseRequest
    {
        public Component ExitPopUpAndSaveTextBox { get; set; }
        public Component Form { get; set; }
        public object Sender { get; set; }
    }
}