using System.ComponentModel;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class MainFormMoveTaskRequest : BaseRequest
    {
        [JsonIgnore]
        public Component TreeView { get; set; }

        public MoveTaskDirection MoveDirection { get; set; }
    }
}