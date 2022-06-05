using System.ComponentModel;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class TreeViewExpandOrCollapseRequest : BaseRequest
{
    public Component TreeView { get; set; }
    public ExpandOrCollapseChangeType ChangeType { get; set; }
}