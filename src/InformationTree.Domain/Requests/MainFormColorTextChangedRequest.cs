using System;
using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class MainFormColorTextChangedRequest : BaseRequest
    {
        public ColorChangeType ChangeType { get; set; }
        public MarshalByRefObject SelectedTreeNode { get; set; }
        public Component ColorTextBox { get; set; }
    }
}