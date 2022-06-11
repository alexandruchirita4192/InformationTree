using InformationTree.Domain.Requests.Base;
using InformationTree.Domain.Responses;

namespace InformationTree.Domain.Services
{
    public interface IProfilingService
    {
        void StartProfiling(BaseRequest request);

        void EndProfiling(BaseRequest request, BaseResponse response);
    }
}