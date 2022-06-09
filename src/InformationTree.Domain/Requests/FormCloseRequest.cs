using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class FormCloseRequest : BaseRequest
    {
        public Component Form { get; set; }
    }
}