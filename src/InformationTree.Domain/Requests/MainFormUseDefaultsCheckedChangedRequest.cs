using System.ComponentModel;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class MainFormUseDefaultsCheckedChangedRequest : BaseRequest
    {
        [JsonIgnore]
        public Component UseDefaultsCheckBox { get; set; }

        [JsonIgnore]
        public Component XNumericUpDown { get; set; }

        [JsonIgnore]
        public Component YNumericUpDown { get; set; }

        [JsonIgnore]
        public Component NumberNumericUpDown { get; set; }

        [JsonIgnore]
        public Component PointsNumericUpDown { get; set; }

        [JsonIgnore]
        public Component RadiusNumericUpDown { get; set; }

        [JsonIgnore]
        public Component ThetaNumericUpDown { get; set; }
    }
}