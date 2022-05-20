using InformationTree.Infrastructure.MediatR.SelfTest.Responses;
using MediatR;

namespace InformationTree.Infrastructure.MediatR.SelfTest.Requests;

public class Ping : IRequest<Pong>
{
    public string Message { get; set; } = string.Empty;
}
