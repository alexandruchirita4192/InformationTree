using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services.Graphics;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormUseDefaultsCheckedChangedHandler : IRequestHandler<MainFormUseDefaultsCheckedChangedRequest, BaseResponse>
    {
        private readonly IGraphicsFileFactory _graphicsFileFactory;

        public MainFormUseDefaultsCheckedChangedHandler(
            IGraphicsFileFactory graphicsFileFactory
            )
        {
            _graphicsFileFactory = graphicsFileFactory;
        }

        public Task<BaseResponse> Handle(MainFormUseDefaultsCheckedChangedRequest request, CancellationToken cancellationToken)
        {
            if (request.UseDefaultsCheckBox is not CheckBox cbUseDefaults)
                return Task.FromResult<BaseResponse>(null);
            if (request.XNumericUpDown is not NumericUpDown nudX)
                return Task.FromResult<BaseResponse>(null);
            if (request.YNumericUpDown is not NumericUpDown nudY)
                return Task.FromResult<BaseResponse>(null);
            if (request.NumberNumericUpDown is not NumericUpDown nudNumber)
                return Task.FromResult<BaseResponse>(null);
            if (request.PointsNumericUpDown is not NumericUpDown nudPoints)
                return Task.FromResult<BaseResponse>(null);
            if (request.ThetaNumericUpDown is not NumericUpDown nudTheta)
                return Task.FromResult<BaseResponse>(null);

            var cbUseDefaultsChecked = cbUseDefaults.Checked;

            nudX.ReadOnly = cbUseDefaultsChecked;
            nudY.ReadOnly = cbUseDefaultsChecked;
            nudNumber.ReadOnly = cbUseDefaultsChecked;
            nudPoints.ReadOnly = cbUseDefaultsChecked;
            nudTheta.ReadOnly = cbUseDefaultsChecked;

            if (cbUseDefaultsChecked)
            {
                nudX.Value = (decimal)_graphicsFileFactory.DefaultX;
                nudY.Value = (decimal)_graphicsFileFactory.DefaultY;
                nudNumber.Value = _graphicsFileFactory.DefaultNumber;
                nudPoints.Value = _graphicsFileFactory.DefaultPoints;
                nudTheta.Value = 0;
            }

            return Task.FromResult(new BaseResponse());
        }
    }
}