using System;
using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class MakeSubTreeWithChildrenOfSelectedNodeRequest : BaseRequest
{
    public bool UseSelectedNode { get; set; }
    public MarshalByRefObject SelectedTreeNode { get; set; }
    public string LinkText { get; set; }
    public Component LinkTextBox { get; set; }
}