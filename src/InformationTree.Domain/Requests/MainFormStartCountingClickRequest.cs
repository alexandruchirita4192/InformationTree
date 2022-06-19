using System;
using System.ComponentModel;
using System.Diagnostics;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class MainFormStartCountingClickRequest : BaseRequest
    {
        [JsonIgnore]
        public Stopwatch Timer { get; set; }

        [JsonIgnore]
        public MarshalByRefObject SelectedNode { get; set; }

        [JsonIgnore]
        public Component TaskGroupBox { get; set; }

        [JsonIgnore]
        public Component TaskListGroupBox { get; set; }
    }
}