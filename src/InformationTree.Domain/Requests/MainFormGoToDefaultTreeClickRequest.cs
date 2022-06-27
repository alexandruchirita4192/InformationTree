using System.ComponentModel;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class MainFormGoToDefaultTreeClickRequest : BaseRequest
    {
        [JsonIgnore]
        public Component ControlToSetWaitCursor { get; set; }

        [JsonIgnore]
        public Component TreeView { get; set; }

        [JsonIgnore]
        public Component ShowUntilNumberNumericUpDown { get; set; }

        [JsonIgnore]
        public Component ShowFromNumberNumericUpDown { get; set; }
    }
}