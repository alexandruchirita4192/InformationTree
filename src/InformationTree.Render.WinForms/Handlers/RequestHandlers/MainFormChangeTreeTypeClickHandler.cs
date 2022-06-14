using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormChangeTreeTypeClickHandler : IRequestHandler<MainFormChangeTreeTypeClickRequest, BaseResponse>
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly IMediator _mediator;
        private readonly ICachingService _cachingService;
        private readonly ISoundProvider _soundProvider;
        private readonly Configuration _configuration;
        private System.Timers.Timer _randomTimer;

        /// <summary>
        /// Elapsed event handler used by MainFormChangeTreeTypeClickRequest
        /// </summary>
        /// <remarks>Handler excessively checked for nulls because it's exposed in a static property</remarks>
        public static ElapsedEventHandler ElapsedEventHandler { get; private set; }

        public MainFormChangeTreeTypeClickHandler(
            IConfigurationReader configurationReader,
            IMediator mediator,
            ICachingService cachingService,
            ISoundProvider soundProvider
            )
        {
            _configurationReader = configurationReader;
            _mediator = mediator;
            _cachingService = cachingService;
            _soundProvider = soundProvider;
            _configuration = _configurationReader.GetConfiguration();

            ElapsedEventHandler = RandomTimer_Elapsed;
        }

        public async Task<BaseResponse> Handle(MainFormChangeTreeTypeClickRequest request, CancellationToken cancellationToken)
        {
            if (request.Timer is not System.Timers.Timer timer)
                return null;
            _randomTimer = timer;

            if (_configuration.ApplicationFeatures.EnableAlerts == false)
                return null;

            if (!_randomTimer.Enabled)
            {
                _randomTimer.Elapsed += RandomTimer_Elapsed; // timer disposed in Dispose(bool)
                RandomTimer_ChangeIntervalAndSound();
                _randomTimer.Start();
            }
            else
            {
                var disposeRequest = new TimerDisposeRequest()
                {
                    Timer = _randomTimer,
                    ElapsedEventHandler = RandomTimer_Elapsed
                };
                await _mediator.Send(disposeRequest, cancellationToken);
            }
            
            return new BaseResponse();
        }

        private void RandomTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_configuration == null)
                return;
            if (_configuration.ApplicationFeatures.EnableAlerts == false)
                return;
            if (_configuration.ApplicationFeatures.EnableExtraSound == false)
                return;

            
            if (_cachingService == null || _soundProvider == null)
            {
                return;
            }
            var soundNumber = _cachingService.Get<int>(Constants.CacheKeys.SoundNumber);
            _soundProvider.PlaySystemSound(soundNumber);
            
            RandomTimer_ChangeIntervalAndSound();
        }

        private void RandomTimer_ChangeIntervalAndSound()
        {
            if (_randomTimer == null || _configuration == null)
                return;
            if (_configuration.ApplicationFeatures.EnableAlerts == false)
                return;

            var ticksSeedAsInt = DateTime.Now
                .Ticks
                .ToInt();

            var interval = 0;
            while (interval == 0)
                interval = (new Random(ticksSeedAsInt).Next() % 10000);

            _randomTimer.Interval = interval;

            var soundNumber = -1;
            while (soundNumber < 1 || soundNumber > 4)
                soundNumber = (new Random(ticksSeedAsInt).Next() % 4) + 1;


            if (_cachingService == null)
                return;
            
            _cachingService.Set(Constants.CacheKeys.SoundNumber, soundNumber);
        }
    }
}