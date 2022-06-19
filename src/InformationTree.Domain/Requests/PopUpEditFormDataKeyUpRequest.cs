using System.ComponentModel;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class PopUpEditFormDataKeyUpRequest : BaseRequest
    {
        [JsonIgnore]
        public Component DataRicherTextBox { get; set; }
        public int KeyData { get; set; }
    }
}