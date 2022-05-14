using InformationTree.Infrastructure.MediatR.Test.Responses;
using MediatR;

namespace InformationTree.Infrastructure.MediatR.Test.Requests;

public class Sing : IStreamRequest<Song>
{
    public string Message { get; set; } = string.Empty;
}
