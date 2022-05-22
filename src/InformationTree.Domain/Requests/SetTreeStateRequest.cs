using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Domain.Requests;

public class SetTreeStateRequest : IRequest<BaseResponse>
{
    public bool? IsSafeToSave { get; set; }
    public int? TreeNodeCounter { get; set; }
    public FileInfo File { get; set; }
}

public class FileInfo
{
    public string FileName { get; set; }
}