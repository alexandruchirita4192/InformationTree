using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Forms;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormShowChildrenAsListToolStripMenuItemClickHandler : IRequestHandler<MainFormShowChildrenAsListToolStripMenuItemClickRequest, BaseResponse>
    {
        public Task<BaseResponse> Handle(MainFormShowChildrenAsListToolStripMenuItemClickRequest request, CancellationToken cancellationToken)
        {
            if (request.SelectedNode is not TreeNode selectedNode)
                return Task.FromResult<BaseResponse>(null);

            var form = new TaskListForm(selectedNode);
            form.ShowDialog();

            return Task.FromResult(new BaseResponse());
        }
    }
}