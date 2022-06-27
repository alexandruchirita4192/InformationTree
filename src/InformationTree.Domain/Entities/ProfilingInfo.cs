using System;
using InformationTree.Domain.Requests.Base;
using InformationTree.Domain.Responses;

namespace InformationTree.Domain.Entities
{
    public class ProfilingInfo
    {
        public BaseRequest Request { get; set; }
        public BaseResponse Response { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration { get; set; }
    }
}