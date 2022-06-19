using System;
using InformationTree.Domain.Requests.Base;
using Newtonsoft.Json;

namespace InformationTree.Domain.Requests
{
    public class LoadingFormIconPaintRequest : BaseRequest
    {
        [JsonIgnore]
        public MarshalByRefObject Graphics { get; set; }
    }
}