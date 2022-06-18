using System;
using System.ComponentModel;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests;

public class TaskListAfterSelectRequest : BaseRequest
{
    [JsonIgnore]
    public Component Form { get; set; }

    [JsonIgnore]
    public Component TreeView { get; set; }

    [JsonIgnore]
    public MarshalByRefObject SelectedNode { get; set; }

    [JsonIgnore]
    public TreeNodeData SelectedNodeData { get; set; }

    public decimal TaskPercentCompleted { get; set; }
    public bool StyleItemCheckEntered { get; set; }

    [JsonIgnore]
    public Component TimeSpentGroupBox { get; set; }

    [JsonIgnore]
    public Component StyleCheckedListBox { get; set; }

    [JsonIgnore]
    public Component FontFamilyComboBox { get; set; }

    [JsonIgnore]
    public Component TaskNameTextBox { get; set; }

    [JsonIgnore]
    public Component AddedDateTextBox { get; set; }

    [JsonIgnore]
    public Component LastChangeDateTextBox { get; set; }

    [JsonIgnore]
    public Component AddedNumberTextBox { get; set; }

    [JsonIgnore]
    public Component UrgencyNumericUpDown { get; set; }

    [JsonIgnore]
    public Component LinkTextBox { get; set; }

    [JsonIgnore]
    public Component IsStartupAlertCheckBox { get; set; }

    [JsonIgnore]
    public Component CompleteProgressNumericUpDown { get; set; }

    [JsonIgnore]
    public Component CategoryTextBox { get; set; }

    [JsonIgnore]
    public Component DataSizeTextBox { get; set; }

    [JsonIgnore]
    public Component HoursNumericUpDown { get; set; }

    [JsonIgnore]
    public Component MinutesNumericUpDown { get; set; }

    [JsonIgnore]
    public Component SecondsNumericUpDown { get; set; }

    [JsonIgnore]
    public Component MillisecondsNumericUpDown { get; set; }

    [JsonIgnore]
    public Component FontSizeNumericUpDown { get; set; }

    [JsonIgnore]
    public Component TextColorTextBox { get; set; }

    [JsonIgnore]
    public Component BackgroundColorTextBox { get; set; }
}