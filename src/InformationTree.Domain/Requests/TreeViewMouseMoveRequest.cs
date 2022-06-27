using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class TreeViewMouseMoveRequest : BaseRequest
{
    public int X { get; set; }
    public int Y { get; set; }

    [JsonIgnore]
    public Component TreeView { get; set; }

    [JsonIgnore]
    public Component Tooltip { get; set; }
}