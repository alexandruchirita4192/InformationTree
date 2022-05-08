using MediatR;

namespace InformationTree.Infrastructure.MediatR.Test.Requests;

public class Jing : IRequest
{
    public string Message { get; set; } = string.Empty;
}
