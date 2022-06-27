using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class TreeViewDeleteRequest : BaseRequest
{
    [JsonIgnore]
    public Component TreeView { get; set; }

    public string TaskNameText { get; set; }

    [JsonIgnore]
    public Component ShowUntilNumberNumericUpDown { get; set; }

    [JsonIgnore]
    public Component ShowFromNumberNumericUpDown { get; set; }

    public TaskListAfterSelectRequest AfterSelectRequest { get; set; }
}