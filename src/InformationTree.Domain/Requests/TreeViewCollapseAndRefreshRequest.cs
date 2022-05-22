using System.ComponentModel;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Domain.Requests;

public class TreeViewCollapseAndRefreshRequest : IRequest<BaseResponse>
{
    public Component TreeView { get; set; }
}