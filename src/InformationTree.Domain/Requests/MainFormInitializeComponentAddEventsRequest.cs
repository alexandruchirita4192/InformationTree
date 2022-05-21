using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class MainFormInitializeComponentAddEventsRequest : BaseRequest
    {
        public Component Form { get; set; }
        public Component TaskListTreeView { get; set; }
        public Component StyleCheckedListBox { get; set; }
        public Component FontFamilyComboBox { get; set; }
        public Component FontSizeNumericUpDown { get; set; }
    }
}