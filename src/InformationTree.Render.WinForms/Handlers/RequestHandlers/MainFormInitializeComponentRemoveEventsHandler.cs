using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Forms;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class MainFormInitializeComponentRemoveEventsHandler
    : IRequestHandler<MainFormInitializeComponentRemoveEventsRequest, BaseResponse>
{
    public Task<BaseResponse> Handle(MainFormInitializeComponentRemoveEventsRequest request, CancellationToken cancellationToken)
    {
        if (request.Form == null
        || request.TaskListTreeView == null
        || request.StyleCheckedListBox == null
        || request.FontFamilyComboBox == null
        || request.FontSizeNumericUpDown == null)
        {
            return Task.FromResult<BaseResponse>(null);
        }

        if (request.Form is MainForm mainForm)
        {
            if (mainForm.IsDisposed)
                return Task.FromResult<BaseResponse>(null);

            mainForm.InvokeWrapper(mainForm => mainForm.MouseWheel -= mainForm.tvTaskList_MouseClick);

            if (request.TaskListTreeView is TreeView tvTaskList)
            {
                tvTaskList.InvokeWrapper(tvTaskList =>
                {
                    tvTaskList.AfterSelect -= mainForm.tvTaskList_AfterSelect;
                    tvTaskList.FontChanged -= mainForm.tvTaskList_FontChanged;
                    tvTaskList.ControlAdded -= mainForm.tvTaskList_ControlAdded;
                    tvTaskList.ControlRemoved -= mainForm.tvTaskList_ControlRemoved;
                    tvTaskList.DoubleClick -= mainForm.tvTaskList_DoubleClick;
                    tvTaskList.KeyDown -= mainForm.tvTaskList_KeyDown;
                    tvTaskList.KeyUp -= mainForm.MainForm_KeyUp;
                    tvTaskList.MouseClick -= mainForm.tvTaskList_MouseClick;
                    tvTaskList.MouseMove -= mainForm.tvTaskList_MouseMove;
                });
            }
            if (request.StyleCheckedListBox is CheckedListBox clbStyle)
                clbStyle.InvokeWrapper(clbStyle => clbStyle.ItemCheck -= mainForm.clbStyle_ItemCheck);
            if (request.FontFamilyComboBox is ComboBox cbFontFamily)
                cbFontFamily.InvokeWrapper(cbFontFamily => cbFontFamily.SelectedIndexChanged -= mainForm.cbFontFamily_SelectedIndexChanged);
            if (request.FontSizeNumericUpDown is NumericUpDown nudFontSize)
                nudFontSize.InvokeWrapper(nudFontSize => nudFontSize.ValueChanged -= mainForm.nudFontSize_ValueChanged);
        }

        return Task.FromResult(new BaseResponse());
    }
}