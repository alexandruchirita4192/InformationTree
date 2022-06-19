using System;
using System.ComponentModel;
using System.Diagnostics;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class MainFormChangeCountingClickRequest : BaseRequest
    {
        [JsonIgnore]
        public Stopwatch Timer { get; set; }

        [JsonIgnore]
        public MarshalByRefObject SelectedNode { get; set; }

        [JsonIgnore]
        public Component TaskGroupBox { get; set; }

        [JsonIgnore]
        public Component TaskListGroupBox { get; set; }

        public ChangeCountingType ChangeCountingType { get; set; }

        [JsonIgnore]
        public Component HoursNumericUpDown { get; set; }

        [JsonIgnore]
        public Component MinutesNumericUpDown { get; set; }

        [JsonIgnore]
        public Component SecondsNumericUpDown { get; set; }

        [JsonIgnore]
        public Component MillisecondsNumericUpDown { get; set; }
    }
}