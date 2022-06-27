using System;
using System.Threading;
using System.Threading.Tasks;
using InformationTree.Domain;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class ShowCanvasPopUpHandler : IRequestHandler<ShowCanvasPopUpRequest, BaseResponse>
{
    private readonly ICanvasFormFactory _canvasFormFactory;
    private readonly ICachingService _cachingService;
    private readonly IConfigurationReader _configurationReader;

    public ShowCanvasPopUpHandler(
        ICanvasFormFactory canvasFormFactory,
        ICachingService cachingService,
        IConfigurationReader configurationReader)
    {
        _canvasFormFactory = canvasFormFactory;
        _cachingService = cachingService;
        _configurationReader = configurationReader;
    }

    public Task<BaseResponse> Handle(ShowCanvasPopUpRequest request, CancellationToken cancellationToken)
    {
        if (_configurationReader.GetConfiguration().ApplicationFeatures.EnableExtraGraphics == false)
            return Task.FromResult<BaseResponse>(null);
        
        var canvasForm = _cachingService.Get<ICanvasForm>(Constants.CacheKeys.CanvasForm);
        if (canvasForm == null || canvasForm.IsDisposed)
        {
            canvasForm = _canvasFormFactory.Create(request.FigureLines);
            _cachingService.Set(Constants.CacheKeys.CanvasForm, canvasForm);
            
            canvasForm.Show();
        }
        else
        {
            canvasForm.RunTimer.Enabled = false;
            try
            {
                canvasForm.GraphicsFile.Clean();
                canvasForm.GraphicsFile.ParseLines(request.FigureLines);
            }
            finally
            {
                canvasForm.RunTimer.Enabled = false;
            }
        }

        return Task.FromResult(new BaseResponse());
    }
}