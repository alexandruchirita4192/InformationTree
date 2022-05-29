using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class TreeViewDoubleClickRequest : BaseRequest
    {
        public Component Form { get; set; }
        public Component TreeView { get; set; }
        public Component TaskNameTextBox { get; set; }
    }
}