using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Domain.Requests;

public class TreeViewCollapseAndRefreshRequest : IRequest<BaseResponse>
{
    [JsonIgnore]
    public Component TreeView { get; set; }
}