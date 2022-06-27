using System.ComponentModel;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class MainFormDoNotSaveClickRequest : BaseRequest
    {
        [JsonIgnore]
        public Component ResetExceptionButton { get; set; }
    }
}