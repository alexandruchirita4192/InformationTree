using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;
using NLog;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormChangeCountingClick : IRequestHandler<MainFormChangeCountingClickRequest, BaseResponse>
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IPopUpService _popUpService;

        public MainFormChangeCountingClick(
            IPopUpService popUpService)
        {
            _popUpService = popUpService;
        }

        public Task<BaseResponse> Handle(MainFormChangeCountingClickRequest request, CancellationToken cancellationToken)
        {
            if (request.SelectedNode is not TreeNode selectedNode)
                return Task.FromResult<BaseResponse>(null);
            if (request.TaskGroupBox is not GroupBox gbTask)
                return Task.FromResult<BaseResponse>(null);
            if (request.TaskListGroupBox is not GroupBox gbTaskList)
                return Task.FromResult<BaseResponse>(null);
            if (request.Timer is not Stopwatch timer)
                return Task.FromResult<BaseResponse>(null);

            if (request.ChangeCountingType == ChangeCountingType.StartCounting)
            {
                timer.Start();

                gbTask.Enabled = false;
                gbTaskList.Enabled = false;
            }
            else if (request.ChangeCountingType == ChangeCountingType.StopCounting)
            {
                if (request.HoursNumericUpDown is not NumericUpDown nudHours)
                    return Task.FromResult<BaseResponse>(null);
                if (request.MinutesNumericUpDown is not NumericUpDown nudMinutes)
                    return Task.FromResult<BaseResponse>(null);
                if (request.SecondsNumericUpDown is not NumericUpDown nudSeconds)
                    return Task.FromResult<BaseResponse>(null);
                if (request.MillisecondsNumericUpDown is not NumericUpDown nudMilliseconds)
                    return Task.FromResult<BaseResponse>(null);

                try
                {
                    timer.Stop();

                    var oldElapsedTime = long.Parse(selectedNode.Name);
                    var elapsedTime = timer.ElapsedMilliseconds;
                    var totalElapsedTime = (oldElapsedTime + elapsedTime);
                    selectedNode.Name = totalElapsedTime.ToString();

                    timer.Reset();

                    var timeSpanTotal = TimeSpan.FromMilliseconds(totalElapsedTime);
                    nudHours.Value = timeSpanTotal.Hours;
                    nudMinutes.Value = timeSpanTotal.Minutes;
                    nudSeconds.Value = timeSpanTotal.Seconds;
                    nudMilliseconds.Value = timeSpanTotal.Milliseconds;
                }
                finally
                {
                    gbTask.Enabled = true;
                    gbTaskList.Enabled = true;
                }
            }

            return Task.FromResult(new BaseResponse());
        }
    }
}