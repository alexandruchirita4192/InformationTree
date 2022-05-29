using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class FontFamilySelectedIndexChangedRequest : BaseRequest
    {
        public Component TreeView { get; set; }
        public Component FontFamilyComboBox { get; set; }
        public Component FontSizeNumericUpDown { get; set; }
    }
}