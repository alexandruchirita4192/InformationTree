using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class TreeViewNoTaskRequest : BaseRequest
    {
        public Component TreeView { get; set; }
        public TaskListAfterSelectRequest AfterSelectRequest { get; set; }
    }
}