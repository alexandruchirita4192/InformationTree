using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class MainFormChangeTreeTypeClickRequest : BaseRequest
    {
        public Component Timer { get; set; }
    }
}