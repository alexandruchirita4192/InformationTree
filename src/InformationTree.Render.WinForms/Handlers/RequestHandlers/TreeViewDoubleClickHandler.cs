﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Forms;
using InformationTree.Render.WinForms.Extensions;
using InformationTree.Render.WinForms.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class TreeViewDoubleClickHandler : IRequestHandler<TreeViewDoubleClickRequest, BaseResponse>
    {
        private readonly ICanvasFormFactory _canvasFormFactory;
        private readonly IPopUpService _popUpService;
        private readonly IConfigurationReader _configurationReader;
        private readonly IMediator _mediator;
        private readonly ICachingService _cachingService;
        private readonly ITreeNodeDataCachingService _treeNodeDataCachingService;

        public TreeViewDoubleClickHandler(
            ICanvasFormFactory canvasFormFactory,
            IPopUpService popUpService,
            IConfigurationReader configurationReader,
            IMediator mediator,
            ICachingService cachingService,
            ITreeNodeDataCachingService treeNodeDataCachingService
            )
        {
            _canvasFormFactory = canvasFormFactory;
            _popUpService = popUpService;
            _configurationReader = configurationReader;
            _mediator = mediator;
            _cachingService = cachingService;
            _treeNodeDataCachingService = treeNodeDataCachingService;
        }

        public Task<BaseResponse> Handle(TreeViewDoubleClickRequest request, CancellationToken cancellationToken)
        {
            if (request.TreeView is not TreeView tvTaskList)
                return Task.FromResult<BaseResponse>(null);
            if (request.Form is not MainForm mainForm)
                return Task.FromResult<BaseResponse>(null);
            if (request.TaskNameTextBox is not TextBox tbTaskName)
                return Task.FromResult<BaseResponse>(null);

            var selectedNode = tvTaskList.SelectedNode;
            if (selectedNode != null)
            {
                var tagData = selectedNode.ToTreeNodeData(_treeNodeDataCachingService);
                var data = tagData.Data ?? string.Empty;

                var form = new PopUpEditForm(
                    _canvasFormFactory,
                    _popUpService,
                    _configurationReader,
                    _mediator,
                    _cachingService,
                    selectedNode.Text,
                    data);

                WinFormsApplication.CenterForm(form, mainForm);

                form.FormClosing += async (s, ev) =>
                {
                    var popUpReturnedData = form.Data;

                    if (selectedNode != null)
                    {
                        var treeNodeData = selectedNode.ToTreeNodeData(_treeNodeDataCachingService);
                        treeNodeData.Data = popUpReturnedData;

                        var strippedData = RicherTextBox.Controls.RicherTextBox.StripRTF(popUpReturnedData);
                        selectedNode.ToolTipText = (selectedNode.Text +
                            (selectedNode.Name.IsNotEmpty() && selectedNode.Name != "0" ? $"{Environment.NewLine} TimeSpent: {selectedNode.Name}" : "") +
                            (strippedData.IsNotEmpty() ? $"{Environment.NewLine} Data: {strippedData}" : ""))
                            .GetToolTipText();

                        tbTaskName.BackColor = tagData.GetTaskNameColor();

                        var setTreeStateRequest = new SetTreeStateRequest
                        {
                            TreeUnchanged = false
                        };
                        await _mediator.Send(setTreeStateRequest, cancellationToken);
                    }
                };

                form.ShowDialog();
            }

            return Task.FromResult(new BaseResponse());
        }
    }
}