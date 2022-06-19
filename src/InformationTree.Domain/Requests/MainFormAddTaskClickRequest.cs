using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class MainFormAddTaskClickRequest : BaseRequest
    {
        public TaskListAfterSelectRequest AfterSelectRequest { get; set; }
        public Component ShowUntilNumberNumericUpDown { get; set; }
        public Component ShowFromNumberNumericUpDown { get; set; }
    }
}