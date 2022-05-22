using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class UpdateNodeCountHandler : IRequestHandler<UpdateNodeCountRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        public UpdateNodeCountHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<BaseResponse> Handle(UpdateNodeCountRequest request, CancellationToken cancellationToken)
        {
            if (request.TreeView is TreeView treeView
            && request.ShowUntilNumberNumericUpDown is NumericUpDown nudShowUntilNumber
            && request.ShowFromNumberNumericUpDown is NumericUpDown nudShowFromNumber)
            {
                treeView.InvokeWrapper(treeView =>
                {
                    var countNodes = treeView.GetNodeCount(true);

                    nudShowUntilNumber.InvokeWrapper(nudShowUntilNumber =>
                    {
                        nudShowUntilNumber.Minimum = 0;
                        nudShowUntilNumber.Maximum = countNodes;
                        nudShowUntilNumber.Value = countNodes;
                    });

                    var setTreeStateRequest = new SetTreeStateRequest
                    {
                        TreeNodeCounter = countNodes
                    };
                    Task.Run(async () =>
                    {
                        return await _mediator.Send(setTreeStateRequest, cancellationToken);
                    }).Wait();

                    nudShowFromNumber.InvokeWrapper(nudShowFromNumber =>
                    {
                        nudShowFromNumber.Minimum = 0;
                        nudShowFromNumber.Maximum = countNodes;
                        nudShowFromNumber.Value = 0;
                    });
                });
            }

            return Task.FromResult(new BaseResponse());
        }
    }
}