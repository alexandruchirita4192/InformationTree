using System;
using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests;

public class MainFormFontSizeValueChangedRequest : BaseRequest
{
    [JsonIgnore]
    public MarshalByRefObject SelectedTreeNode { get; set; }

    [JsonIgnore]
    public Component FontSizeNumericUpDown { get; set; }
}