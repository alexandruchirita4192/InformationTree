using System.ComponentModel;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests;

public class FormKeyUpRequest : BaseRequest
{
    [JsonIgnore]
    public Component Form { get; set; }

    public int KeyData { get; set; }
}