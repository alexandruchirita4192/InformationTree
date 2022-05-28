using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class TreeViewDeleteRequest : BaseRequest
    {
        public Component TreeView { get; set; }
        public string TaskNameText { get; set; }
        public Component ShowUntilNumberNumericUpDown { get; set; }
        public Component ShowFromNumberNumericUpDown { get; set; }
        public TaskListAfterSelectRequest AfterSelectRequest { get; set; }
    }
}