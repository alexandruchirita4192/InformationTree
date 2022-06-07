using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class FormMouseDoubleClickRequest : BaseRequest
    {
        public Component Form { get; set; }
    }
}