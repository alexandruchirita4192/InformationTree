using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormStyleItemCheckHandler : IRequestHandler<MainFormStyleItemCheckRequest, BaseResponse>
    {
        private readonly ICachingService _cachingService;
        private readonly IMediator _mediator;
        private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;

        public MainFormStyleItemCheckHandler(
            ICachingService cachingService,
            IMediator mediator,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter
            )
        {
            _cachingService = cachingService;
            _mediator = mediator;
            _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
        }

        public async Task<BaseResponse> Handle(MainFormStyleItemCheckRequest request, CancellationToken cancellationToken)
        {
            if (request.ItemCheckEventArgs is not ItemCheckEventArgs itemCheckEventArgs)
                return null;
            if (request.AfterSelectRequest == null)
                return null;
            if (request.AfterSelectRequest.TreeView is not TreeView tvTaskList)
                return null;
            if (request.AfterSelectRequest.StyleCheckedListBox is not CheckedListBox clbStyle)
                return null;

            var _clbStyle_ItemCheckEntered = _cachingService.Get<bool>(Constants.CacheKeys.StyleCheckedListBox_ItemCheckEntered);
            if (_clbStyle_ItemCheckEntered)
                return null;

            try
            {
                _cachingService.Set(Constants.CacheKeys.StyleCheckedListBox_ItemCheckEntered, true);

                if (tvTaskList.SelectedNode != null)
                {
                    var newFontStyle = FontStyle.Regular;

                    AppendTheCheckedFontStyleToCurrentFontStyle(itemCheckEventArgs, clbStyle, ref newFontStyle, FontStyle.Italic);
                    AppendTheCheckedFontStyleToCurrentFontStyle(itemCheckEventArgs, clbStyle, ref newFontStyle, FontStyle.Bold);
                    AppendTheCheckedFontStyleToCurrentFontStyle(itemCheckEventArgs, clbStyle, ref newFontStyle, FontStyle.Underline);
                    AppendTheCheckedFontStyleToCurrentFontStyle(itemCheckEventArgs, clbStyle, ref newFontStyle, FontStyle.Strikeout);

                    var font = tvTaskList.SelectedNode.NodeFont ?? tvTaskList.SelectedNode.Parent?.NodeFont ?? WinFormsConstants.FontDefaults.DefaultFont;
                    if (font != null)
                        tvTaskList.SelectedNode.NodeFont = new Font(font.FontFamily, font.Size, newFontStyle);

                    var backColor = tvTaskList.SelectedNode.BackColor;
                    tvTaskList.SelectedNode.BackColor = backColor != null && !backColor.IsEmpty ? backColor : Constants.Colors.DefaultBackGroundColor;
                    var foreColor = tvTaskList.SelectedNode.ForeColor;
                    tvTaskList.SelectedNode.ForeColor = foreColor != null && !foreColor.IsEmpty ? foreColor : Constants.Colors.DefaultForeGroundColor;

                    var nodeData = _treeNodeToTreeNodeDataAdapter.Adapt(tvTaskList.SelectedNode);
                    nodeData.LastChangeDate = DateTime.Now;

                    await _mediator.Send(request.AfterSelectRequest, cancellationToken);

                    // on font changed is added too??
                    var setTreeStateRequest = new SetTreeStateRequest
                    {
                        TreeUnchanged = false
                    };
                    return await _mediator.Send(setTreeStateRequest, cancellationToken);
                }
            }
            finally
            {
                _cachingService.Set(Constants.CacheKeys.StyleCheckedListBox_ItemCheckEntered, false);
            }

            return null;
        }

        private static FontStyle AppendTheCheckedFontStyleToCurrentFontStyle(ItemCheckEventArgs e, CheckedListBox clb, ref FontStyle currentFontStyle, FontStyle checkedFontStyle)
        {
            if (e == null || clb == null)
                return currentFontStyle;

            var idx = clb.Items.IndexOf(checkedFontStyle);
            var checkedState = idx == e.Index ? e.NewValue : clb.GetItemCheckState(idx);
            if (checkedState == CheckState.Checked)
                currentFontStyle |= checkedFontStyle;
            return currentFontStyle;
        }
    }
}