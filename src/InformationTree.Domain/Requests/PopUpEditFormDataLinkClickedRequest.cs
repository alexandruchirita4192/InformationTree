using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class PopUpEditFormDataLinkClickedRequest : BaseRequest
    {
        public string LinkText { get; set; }
    }
}