using System.ComponentModel;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class SearchFormFindKeyUpRequest : BaseRequest
    {
        public Component Form { get; set; }
        public int KeyData { get; set; }
        public Component FindTextBox { get; set; }
    }
}