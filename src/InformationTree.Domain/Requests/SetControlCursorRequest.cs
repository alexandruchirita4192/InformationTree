using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Domain.Requests
{
    public class SetControlCursorRequest : IRequest<BaseResponse>
    {
        [JsonIgnore]
        public Component Control { get; set; }

        public bool IsWaitCursor { get; set; }
    }
}