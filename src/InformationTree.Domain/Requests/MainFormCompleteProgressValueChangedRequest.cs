using System;
using System.ComponentModel;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class MainFormCompleteProgressValueChangedRequest : BaseRequest
    {
        [JsonIgnore]
        public Component PercentCompleteProgressBar { get; set; }

        [JsonIgnore]
        public Component CompleteProgressNumericUpDown { get; set; }

        [JsonIgnore]
        public MarshalByRefObject SelectedNode { get; set; }
    }
}