using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class ShowStartupAlertFormRequest : BaseRequest
{
    [JsonIgnore]
    public Component TreeView { get; set; }

    [JsonIgnore]
    public Component SearchBoxTextBox { get; set; }
}