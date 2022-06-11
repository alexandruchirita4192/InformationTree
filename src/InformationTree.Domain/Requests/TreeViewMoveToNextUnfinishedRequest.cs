using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class TreeViewMoveToNextUnfinishedRequest : BaseRequest
    {
        [JsonIgnore]
        public Component TreeView { get; set; }
    }
}