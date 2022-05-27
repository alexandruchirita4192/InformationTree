using System;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class CalculatePercentageRequest : BaseRequest
    {
        public MarshalByRefObject SelectedNode { get; set; }
        public CalculatePercentageDirection Direction { get; set; }
    }
}