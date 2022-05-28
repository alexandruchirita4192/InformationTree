using System;
using System.ComponentModel;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class TaskListAfterSelectRequest : BaseRequest
    {
        public Component Form { get; set; }
        public Component TreeView { get; set; }
        public MarshalByRefObject SelectedNode { get; set; }
        public TreeNodeData SelectedNodeData { get; set; }
        public decimal TaskPercentCompleted { get; set; }
        public bool StyleItemCheckEntered { get; set; }
        public Component TimeSpentGroupBox { get; set; }
        public Component StyleCheckedListBox { get; set; }
        public Component FontFamilyComboBox { get; set; }
        public Component TaskNameTextBox { get; set; }
        public Component AddedDateTextBox { get; set; }
        public Component LastChangeDateTextBox { get; set; }
        public Component AddedNumberTextBox { get; set; }
        public Component UrgencyNumericUpDown { get; set; }
        public Component LinkTextBox { get; set; }
        public Component IsStartupAlertCheckBox { get; set; }
        public Component CompleteProgressNumericUpDown { get; set; }
        public Component CategoryTextBox { get; set; }
        public Component DataSizeTextBox { get; set; }
        public Component HoursNumericUpDown { get; set; }
        public Component MinutesNumericUpDown { get; set; }
        public Component SecondsNumericUpDown { get; set; }
        public Component MillisecondsNumericUpDown { get; set; }
        public Component FontSizeNumericUpDown { get; set; }
        public Component TextColorTextBox { get; set; }
        public Component BackgroundColorTextBox { get; set; }
    }
}