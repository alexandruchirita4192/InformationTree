using System.ComponentModel;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class MainFormTreeViewKeyDownRequest : BaseRequest
    {
        public TaskListAfterSelectRequest AfterSelectRequest { get; set; }
        
        public int KeyData { get; set; }

        [JsonIgnore]
        public Component SearchBoxTextBox { get; set; }

        [JsonIgnore]
        public object Sender { get; set; }

        [JsonIgnore]
        public Component ShowUntilNumberNumericUpDown { get; set; }

        [JsonIgnore]
        public Component ShowFromNumberNumericUpDown { get; set; }
    }
}