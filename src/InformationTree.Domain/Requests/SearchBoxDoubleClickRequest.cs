using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class SearchBoxDoubleClickRequest : BaseRequest
{
    [JsonIgnore]
    public Component SearchBoxTextBox { get; set; }

    [JsonIgnore]
    public Component Form { get; set; }

    [JsonIgnore]
    public Component TreeView { get; set; }
}