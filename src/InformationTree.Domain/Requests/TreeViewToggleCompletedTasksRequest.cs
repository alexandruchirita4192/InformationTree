using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class TreeViewToggleCompletedTasksRequest : BaseRequest
{
    [JsonIgnore]
    public Component TreeView { get; set; }
}