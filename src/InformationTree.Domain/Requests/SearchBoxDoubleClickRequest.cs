using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class SearchBoxDoubleClickRequest : BaseRequest
    {
        public Component SearchBoxTextBox { get; set; }
        public Component Form { get; set; }
        public Component TreeView { get; set; }
    }
}