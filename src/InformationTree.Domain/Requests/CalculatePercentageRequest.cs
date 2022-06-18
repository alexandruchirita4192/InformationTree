using System;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests;

public class CalculatePercentageRequest : BaseRequest
{
    [JsonIgnore]
    public MarshalByRefObject SelectedNode { get; set; }

    public CalculatePercentageDirection Direction { get; set; }
}