using System.ComponentModel;
using Newtonsoft.Json;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Domain.Requests;

public class UpdateNodeCountRequest : IRequest<BaseResponse>
{
    [JsonIgnore]
    public Component TreeView { get; set; }

    [JsonIgnore]
    public Component ShowUntilNumberNumericUpDown { get; set; }

    [JsonIgnore]
    public Component ShowFromNumberNumericUpDown { get; set; }
}