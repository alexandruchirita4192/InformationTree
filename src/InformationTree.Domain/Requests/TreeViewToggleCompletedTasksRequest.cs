using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class TreeViewToggleCompletedTasksRequest : BaseRequest
    {
        public Component TreeView { get; set; }
    }
}