using System.ComponentModel;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Domain.Requests
{
    public class SetControlCursorRequest : IRequest<BaseResponse>
    {
        public Component Control { get; set; }
        public bool IsWaitCursor { get; set; }
    }
}