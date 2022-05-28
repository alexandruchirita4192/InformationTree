using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Render.WinForms.Extensions;
using InformationTree.TextProcessing;
using InformationTree.Tree;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class TaskListAfterSelectHandler : IRequestHandler<TaskListAfterSelectRequest, BaseResponse>
{
    private readonly IMediator _mediator;
    private readonly ITreeNodeDataCachingService _treeNodeDataCachingService;
    private readonly ITreeNodeSelectionCachingService _treeNodeSelectionCachingService;

    public TaskListAfterSelectHandler(
        IMediator mediator,
        ITreeNodeDataCachingService treeNodeDataCachingService,
        ITreeNodeSelectionCachingService treeNodeSelectionCachingService)
    {
        _mediator = mediator;
        _treeNodeDataCachingService = treeNodeDataCachingService;
        _treeNodeSelectionCachingService = treeNodeSelectionCachingService;
    }

    public async Task<BaseResponse> Handle(TaskListAfterSelectRequest request, CancellationToken cancellationToken)
    {
        var treeNodeData = request.SelectedNodeData;
        if (treeNodeData == null || request.Form == null)
            return null;

        try
        {
            var removeEventsRequest = new MainFormInitializeComponentRemoveEventsRequest
            {
                Form = request.Form,
                TaskListTreeView = request.TreeView,
                StyleCheckedListBox = request.StyleCheckedListBox,
                FontFamilyComboBox = request.FontFamilyComboBox,
                FontSizeNumericUpDown = request.FontSizeNumericUpDown
            };
            await _mediator.Send(removeEventsRequest, cancellationToken);

            var percentCompleted = request.TaskPercentCompleted;
            var timeSpanTotal = treeNodeData.Name.IsNotEmpty() && long.TryParse(treeNodeData.Name, out long timeSpanMilliseconds)
                ? TimeSpan.FromMilliseconds(timeSpanMilliseconds)
                : new TimeSpan(0);

            if (request.TaskNameTextBox is TextBox tbTaskName)
            {
                tbTaskName.InvokeWrapper(tbTaskName =>
                {
                    tbTaskName.Text = TextProcessingHelper.GetTextAndProcentCompleted(treeNodeData.Text, ref percentCompleted, true);
                    tbTaskName.BackColor = treeNodeData.GetTaskNameColor();
                    treeNodeData.PercentCompleted = percentCompleted;
                });
            }
            if (request.AddedDateTextBox is TextBox tbAddedDate)
                tbAddedDate.InvokeWrapper(tbAddedDate => tbAddedDate.Text = treeNodeData.AddedDate.HasValue ? treeNodeData.AddedDate.Value.ToFormattedString() : "-");
            if (request.LastChangeDateTextBox is TextBox tbLastChangeDate)
                tbLastChangeDate.InvokeWrapper(tbLastChangeDate => tbLastChangeDate.Text = treeNodeData.LastChangeDate.HasValue ? treeNodeData.LastChangeDate.Value.ToFormattedString() : "-");
            if (request.AddedNumberTextBox is TextBox tbAddedNumber)
                tbAddedNumber.InvokeWrapper(tbAddedNumber => tbAddedNumber.Text = treeNodeData.AddedNumber.ToString());
            if (request.UrgencyNumericUpDown is NumericUpDown nudUrgency)
                nudUrgency.InvokeWrapper(nudUrgency => nudUrgency.Value = treeNodeData.Urgency);
            if (request.LinkTextBox is TextBox tbLink)
                tbLink.InvokeWrapper(tbLink => tbLink.Text = treeNodeData.Link);
            if (request.IsStartupAlertCheckBox is CheckBox cbIsStartupAlert)
                cbIsStartupAlert.InvokeWrapper(cbIsStartupAlert => cbIsStartupAlert.Checked = treeNodeData.IsStartupAlert);
            if (request.CompleteProgressNumericUpDown is NumericUpDown nudCompleteProgress)
                nudCompleteProgress.InvokeWrapper(nudCompleteProgress => nudCompleteProgress.Value = percentCompleted);

            if (request.CategoryTextBox is TextBox tbCategory)
                tbCategory.Text = treeNodeData.Category;
            if (request.DataSizeTextBox is TextBox tbDataSize)
            {
                var sizeBytes = CalculateDataSizeFromNodeAndChildren(treeNodeData, _treeNodeDataCachingService);
                var sizeMb = sizeBytes / 1024 / 1024;
                tbDataSize.InvokeWrapper(tbDataSize => tbDataSize.Text = $"{sizeBytes}b {sizeMb}M");
            }
            if (request.HoursNumericUpDown is NumericUpDown nudHours)
                nudHours.InvokeWrapper(nudHours => nudHours.Value = timeSpanTotal.Hours);
            if (request.MinutesNumericUpDown is NumericUpDown nudMinutes)
                nudMinutes.InvokeWrapper(nudMinutes => nudMinutes.Value = timeSpanTotal.Minutes);
            if (request.SecondsNumericUpDown is NumericUpDown nudSeconds)
                nudSeconds.InvokeWrapper(nudSeconds => nudSeconds.Value = timeSpanTotal.Seconds);
            if (request.MillisecondsNumericUpDown is NumericUpDown nudMilliseconds)
                nudMilliseconds.InvokeWrapper(nudMilliseconds => nudMilliseconds.Value = timeSpanTotal.Milliseconds);

            if (request.TimeSpentGroupBox is GroupBox gbTimeSpent)
                gbTimeSpent.InvokeWrapper(gbTimeSpent => gbTimeSpent.Enabled = true);

            if (treeNodeData.NodeFont != null)
            {
                if (request.FontSizeNumericUpDown is NumericUpDown nudFontSize)
                {
                    nudFontSize.InvokeWrapper(nudFontSize =>
                    {
                        var defaultSize = 8;
                        var size = ((decimal)treeNodeData.NodeFont.Size) > nudFontSize.Maximum
                            ? defaultSize
                            : ((decimal)treeNodeData.NodeFont.Size);
                        size = size < nudFontSize.Maximum ? defaultSize : size;
                        nudFontSize.Value = size;
                    });
                }

                if (!request.StyleItemCheckEntered)
                {
                    if (request.StyleCheckedListBox is CheckedListBox clbStyle)
                    {
                        UpdateCheckedListBoxBasedOnFont(true, clbStyle, FontStyle.Regular);
                        UpdateCheckedListBoxBasedOnFont(treeNodeData.NodeFont.Italic, clbStyle, FontStyle.Italic);
                        UpdateCheckedListBoxBasedOnFont(treeNodeData.NodeFont.Bold, clbStyle, FontStyle.Bold);
                        UpdateCheckedListBoxBasedOnFont(treeNodeData.NodeFont.Strikeout, clbStyle, FontStyle.Strikeout);
                        UpdateCheckedListBoxBasedOnFont(treeNodeData.NodeFont.Underline, clbStyle, FontStyle.Underline);
                    }

                    if (request.FontFamilyComboBox is ComboBox cbFontFamily)
                    {
                        cbFontFamily.InvokeWrapper(cbFontFamily =>
                        {
                            // Update font family based on current font
                            if (treeNodeData.NodeFont.FontFamilyName.IsNotEmpty() && cbFontFamily.Items.Count != 0)
                                for (int i = 0; i < cbFontFamily.Items.Count; i++)
                                {
                                    if (cbFontFamily.Items[i] is string fontFamily
                                        && string.Compare(fontFamily, treeNodeData.NodeFont.FontFamilyName, StringComparison.InvariantCultureIgnoreCase) == 0)
                                    {
                                        cbFontFamily.SelectedIndex = i;
                                        break;
                                    }
                                }
                        });
                    }

                    if (request.TextColorTextBox is TextBox tbTextColor)
                        tbTextColor.InvokeWrapper(tbTextColor => tbTextColor.Text = treeNodeData.ForeColorName);
                    if (request.BackgroundColorTextBox is TextBox tbBackgroundColor)
                        tbBackgroundColor.InvokeWrapper(tbBackgroundColor => tbBackgroundColor.Text = treeNodeData.BackColorName);
                }
            }

            if (request.SelectedNode is TreeNode treeNode)
                _treeNodeSelectionCachingService.AddToCache(treeNode);

            return new BaseResponse();
        }
        finally
        {
            var addEventsRequest = new MainFormInitializeComponentAddEventsRequest
            {
                Form = request.Form,
                TaskListTreeView = request.TreeView,
                StyleCheckedListBox = request.StyleCheckedListBox,
                FontFamilyComboBox = request.FontFamilyComboBox,
                FontSizeNumericUpDown = request.FontSizeNumericUpDown
            };
            await _mediator.Send(addEventsRequest, cancellationToken);
        }
    }

    private void UpdateCheckedListBoxBasedOnFont(bool itemChecked, CheckedListBox clb, FontStyle fontStyle)
    {
        if (clb == null)
            return;
        clb.InvokeWrapper((clb) =>
        {
            int idx = clb.Items.IndexOf(fontStyle);
            var checkedState = ((!clb.GetItemChecked(idx)) && itemChecked) || fontStyle == FontStyle.Regular ? CheckState.Checked : CheckState.Unchecked;
            clb.SetItemCheckState(idx, checkedState);
        });
    }

    private int CalculateDataSizeFromNodeAndChildren(TreeNodeData tagData, ITreeNodeDataCachingService treeNodeDataCachingService)
    {
        if (tagData == null)
            return 0;
        if (treeNodeDataCachingService == null)
            throw new ArgumentNullException(nameof(treeNodeDataCachingService));

        var size = tagData.Data.IsEmpty() ? 0 : tagData.Data.Length;
        foreach (TreeNodeData nd in tagData.Children)
            size += CalculateDataSizeFromNodeAndChildren(nd, treeNodeDataCachingService);
        return size;
    }
}