using System;
using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class MakeSubTreeWithChildrenOfSelectedNodeRequest : BaseRequest
{
    public bool UseSelectedNode { get; set; }

    [JsonIgnore]
    public MarshalByRefObject SelectedTreeNode { get; set; }

    public string LinkText { get; set; }

    [JsonIgnore]
    public Component LinkTextBox { get; set; }
}