using System.ComponentModel;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests;

public class MainFormChangeTreeTypeClickRequest : BaseRequest
{
    [JsonIgnore]
    public Component Timer { get; set; }
}