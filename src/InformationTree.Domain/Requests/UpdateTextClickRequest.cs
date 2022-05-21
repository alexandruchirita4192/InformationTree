using System.ComponentModel;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class UpdateTextClickRequest : BaseRequest
    {
        public TreeNodeData SelectedNode { get; set; }
        public decimal TaskPercentCompleted { get; set; }
        public string TaskName { get; set; }
        public string Link { get; set; }
        public int Urgency { get; set; }
        public string Category { get; set; }
        public bool IsStartupAlert { get; set; }
        public Component TaskListTreeView { get; set; }
        public TaskListAfterSelectRequest AfterSelectRequest { get; set; }
    }
}