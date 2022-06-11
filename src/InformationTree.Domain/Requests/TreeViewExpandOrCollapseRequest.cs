using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class TreeViewExpandOrCollapseRequest : BaseRequest
{
    [JsonIgnore]
    public Component TreeView { get; set; }

    public ExpandOrCollapseChangeType ChangeType { get; set; }
}