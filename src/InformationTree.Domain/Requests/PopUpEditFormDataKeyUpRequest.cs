using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class PopUpEditFormDataKeyUpRequest : BaseRequest
    {
        public Component DataRicherTextBox { get; set; }
        public int KeyData { get; set; }
    }
}