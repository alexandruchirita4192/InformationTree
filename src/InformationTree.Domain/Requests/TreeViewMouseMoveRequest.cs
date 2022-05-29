using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class TreeViewMouseMoveRequest : BaseRequest
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Component TreeView { get; set; }
        public Component Tooltip { get; set; }
    }
}