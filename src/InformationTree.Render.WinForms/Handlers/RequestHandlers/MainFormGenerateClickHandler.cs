using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Domain;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormGenerateClickHandler : IRequestHandler<MainFormGenerateClickRequest, BaseResponse>
    {
        private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;
        private readonly IMediator _mediator;
        private readonly IGraphicsFileFactory _graphicsFileFactory;

        public MainFormGenerateClickHandler(
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter,
            IMediator mediator,
            IGraphicsFileFactory graphicsFileFactory
            )
        {
            _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
            _mediator = mediator;
            _graphicsFileFactory = graphicsFileFactory;
        }

        public async Task<BaseResponse> Handle(MainFormGenerateClickRequest request, CancellationToken cancellationToken)
        {
            if (request.SelectedNode is not TreeNode selectedNode)
                return null;
            if (request.XNumericUpDown is not NumericUpDown nudX)
                return null;
            if (request.YNumericUpDown is not NumericUpDown nudY)
                return null;
            if (request.NumberNumericUpDown is not NumericUpDown nudNumber)
                return null;
            if (request.PointsNumericUpDown is not NumericUpDown nudPoints)
                return null;
            if (request.RadiusNumericUpDown is not NumericUpDown nudRadius)
                return null;
            if (request.ThetaNumericUpDown is not NumericUpDown nudTheta)
                return null;
            if (request.IterationsNumericUpDown is not NumericUpDown nudIterations)
                return null;
            if (request.UseDefaultsCheckBox is not CheckBox cbUseDefaults)
                return null;
            if (request.ComputeTypeNumericUpDown is not NumericUpDown nudComputeType)
                return null;
            if (request.LogCheckBox is not CheckBox cbLog)
                return null;
            if (request.CommandTextBox is not TextBox tbCommand)
                return null;

            var x = (double)nudX.Value;
            var y = (double)nudY.Value;
            var number = (int)nudNumber.Value;
            var points = (int)nudPoints.Value;
            var radius = (double)nudRadius.Value;
            var theta = (double)nudTheta.Value;
            var iterations = (int)nudIterations.Value;

            var cbUseDefaultsChecked = cbUseDefaults.Checked;
            var computeType = (ComputeType)nudComputeType.Value;
            var computeTypeInt = (int)computeType;

            var cbLogChecked = cbLog.Checked;
            var commandData = cbUseDefaultsChecked ?
                $"{GraphicsFileConstants.GenerateFigureLines.DefaultName} {radius} {theta} {iterations} {computeTypeInt}" :
                $"{GraphicsFileConstants.GenerateFigureLines.DefaultName} {points} {x} {y} {radius} {theta} {number} {iterations} {computeTypeInt}";

            if (cbLogChecked && selectedNode != null)
            {
                var selectedTreeNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(selectedNode);
                selectedTreeNodeData.Data += commandData + Environment.NewLine;

                var setTreeStateRequest = new SetTreeStateRequest
                {
                    TreeUnchanged = false
                };
                await _mediator.Send(setTreeStateRequest, cancellationToken);
            }

            tbCommand.Lines = cbUseDefaultsChecked
                ? _graphicsFileFactory.GenerateFigureLines(radius, iterations, computeType)
                    .Distinct()
                    .ToArray()
                : _graphicsFileFactory.GenerateFigureLines(points, x, y, radius, theta, number, iterations, computeType)
                    .Distinct()
                    .ToArray();

            return new BaseResponse();
        }
    }
}