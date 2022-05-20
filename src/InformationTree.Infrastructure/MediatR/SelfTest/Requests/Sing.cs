using InformationTree.Infrastructure.MediatR.SelfTest.Responses;
using MediatR;

namespace InformationTree.Infrastructure.MediatR.SelfTest.Requests;

public class Sing : IStreamRequest<Song>
{
    public string Message { get; set; } = string.Empty;
}
