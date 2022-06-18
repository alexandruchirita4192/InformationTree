using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class TimerDisposeHandler : IRequestHandler<TimerDisposeRequest, BaseResponse>
{
    public Task<BaseResponse> Handle(TimerDisposeRequest request, CancellationToken cancellationToken)
    {
        if (request.Timer is not System.Timers.Timer timer)
            return Task.FromResult<BaseResponse>(null);

        timer.Stop();

        if (request.ElapsedEventHandler != null)
            timer.Elapsed -= request.ElapsedEventHandler;

        return Task.FromResult(new BaseResponse());
    }
}