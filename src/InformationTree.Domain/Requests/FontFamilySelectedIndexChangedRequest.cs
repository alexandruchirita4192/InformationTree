using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class FontFamilySelectedIndexChangedRequest : BaseRequest
    {
        [JsonIgnore]
        public Component TreeView { get; set; }

        [JsonIgnore]
        public Component FontFamilyComboBox { get; set; }

        [JsonIgnore]
        public Component FontSizeNumericUpDown { get; set; }
    }
}