using InformationTree.Infrastructure.MediatR.Test.Responses;
using MediatR;

namespace InformationTree.Infrastructure.MediatR.Test.Requests;

public class Ping : IRequest<Pong>
{
    public string Message { get; set; } = string.Empty;
}
