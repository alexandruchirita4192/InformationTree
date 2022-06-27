using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class ShowTreeFilteredByRangeRequest : BaseRequest
{
    public int Min { get; set; }
    public int Max { get; set; }
    public CopyNodeFilterType FilterType { get; set; }

    [JsonIgnore]
    public Component TreeView { get; set; }

    [JsonIgnore]
    public List<Component> ControlsToEnableForFiltered { get; set; } // ShowAllButton to restore state

    [JsonIgnore]
    public List<Component> ControlsToEnableForNotFiltered { get; set; } // Many controls that change existing tree state
}