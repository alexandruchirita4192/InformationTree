using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class TreeViewDoubleClickRequest : BaseRequest
{
    [JsonIgnore]
    public Component Form { get; set; }

    [JsonIgnore]
    public Component TreeView { get; set; }

    [JsonIgnore]
    public Component TaskNameTextBox { get; set; }
}