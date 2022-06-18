using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class TreeViewNoTaskRequest : BaseRequest
{
    [JsonIgnore]
    public Component TreeView { get; set; }

    public TaskListAfterSelectRequest AfterSelectRequest { get; set; }
}