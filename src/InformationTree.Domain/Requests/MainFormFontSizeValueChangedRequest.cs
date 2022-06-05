using System;
using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class MainFormFontSizeValueChangedRequest : BaseRequest
    {
        public MarshalByRefObject SelectedTreeNode { get; set; }
        public Component FontSizeNumericUpDown { get; set; }
    }
}