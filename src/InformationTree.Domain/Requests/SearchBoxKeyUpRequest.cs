using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class SearchBoxKeyUpRequest : BaseRequest
    {
        public Component SearchBoxTextBox { get; set; }
        public Component TreeView { get; set; }
        public int KeyData { get; set; }
    }
}