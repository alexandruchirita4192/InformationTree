using MediatR;

namespace InformationTree.Infrastructure.MediatR.SelfTest.Requests;

public class Jing : IRequest
{
    public string Message { get; set; } = string.Empty;
}
