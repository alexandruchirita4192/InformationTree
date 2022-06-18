using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class FormCloseRequest : BaseRequest
{
    [JsonIgnore]
    public Component Form { get; set; }
}