using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class ShowStartupAlertFormRequest : BaseRequest
    {
        public Component TreeView { get; set; }
        public Component SearchBoxTextBox { get; set; }
    }
}