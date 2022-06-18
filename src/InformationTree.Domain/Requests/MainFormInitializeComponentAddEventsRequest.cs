using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class MainFormInitializeComponentAddEventsRequest : BaseRequest
{
    [JsonIgnore]
    public Component Form { get; set; }

    [JsonIgnore]
    public Component TaskListTreeView { get; set; }

    [JsonIgnore]
    public Component StyleCheckedListBox { get; set; }

    [JsonIgnore]
    public Component FontFamilyComboBox { get; set; }

    [JsonIgnore]
    public Component FontSizeNumericUpDown { get; set; }
}