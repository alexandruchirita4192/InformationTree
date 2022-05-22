using System.ComponentModel;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Domain.Requests;

public class UpdateNodeCountRequest : IRequest<BaseResponse>
{
    public Component TreeView { get; set; }
    public Component ShowUntilNumberNumericUpDown { get; set; }
    public Component ShowFromNumberNumericUpDown { get; set; }
}