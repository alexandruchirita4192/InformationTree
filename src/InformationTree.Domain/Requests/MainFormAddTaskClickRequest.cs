using System.ComponentModel;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class MainFormAddTaskClickRequest : BaseRequest
    {
        public TaskListAfterSelectRequest AfterSelectRequest { get; set; }

        [JsonIgnore]
        public Component ShowUntilNumberNumericUpDown { get; set; }

        [JsonIgnore]
        public Component ShowFromNumberNumericUpDown { get; set; }
    }
}