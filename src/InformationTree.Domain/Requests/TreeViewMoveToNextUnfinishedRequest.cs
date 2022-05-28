using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class TreeViewMoveToNextUnfinishedRequest : BaseRequest
    {
        public Component TreeView { get; set; }
    }
}