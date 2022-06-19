using System;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Domain.Requests
{
    public class LoadingFormIconPaintRequest : BaseRequest
    {
        public MarshalByRefObject Graphics { get; set; }
    }
}