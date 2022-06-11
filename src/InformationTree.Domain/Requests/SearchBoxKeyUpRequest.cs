using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class SearchBoxKeyUpRequest : BaseRequest
    {
        [JsonIgnore]
        public Component SearchBoxTextBox { get; set; }

        [JsonIgnore]
        public Component TreeView { get; set; }

        public int KeyData { get; set; }
    }
}