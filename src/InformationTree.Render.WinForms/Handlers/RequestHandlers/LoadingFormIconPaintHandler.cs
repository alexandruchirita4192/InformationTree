using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class LoadingFormIconPaintHandler : IRequestHandler<LoadingFormIconPaintRequest, BaseResponse>
    {
        public Task<BaseResponse> Handle(LoadingFormIconPaintRequest request, CancellationToken cancellationToken)
        {
            if (request.Graphics is not Graphics graphics)
                return Task.FromResult<BaseResponse>(null);

            var currentAssembly = Assembly.GetEntryAssembly();
            if (currentAssembly == null)
                return Task.FromResult<BaseResponse>(null);

            var currentAssemblyLocation = currentAssembly.Location;
            if (currentAssemblyLocation.IsEmpty())
                return Task.FromResult<BaseResponse>(null);

            var extractedIconFromCurrentAssembly = Icon.ExtractAssociatedIcon(currentAssemblyLocation);
            if (extractedIconFromCurrentAssembly == null)
                return Task.FromResult<BaseResponse>(null);

            graphics.DrawIcon(extractedIconFromCurrentAssembly, 0, 0);

            return Task.FromResult(new BaseResponse());
        }
    }
}