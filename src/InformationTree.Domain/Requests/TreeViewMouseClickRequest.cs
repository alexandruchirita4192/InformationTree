using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class TreeViewMouseClickRequest : BaseRequest
    {
        public bool IsControlPressed { get; set; }
        public int MouseDelta { get; set; }
        public Component TreeView { get; set; }
    }
}