using System;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Domain.Requests;

public class SetTreeStateRequest : IRequest<BaseResponse>
{
    public bool? IsSafeToSave { get; set; }
    public int? TreeNodeCounter { get; set; }
    public bool? TreeUnchanged { get; set; }
    public bool? TreeSaved { get; set; }
    public DateTime? TreeSavedAt { get; set; }
    public bool? ReadOnlyState { get; set; }
    public FileInfo FileInfo { get; set; }
}

public class FileInfo
{
    public string FileName { get; set; }
}