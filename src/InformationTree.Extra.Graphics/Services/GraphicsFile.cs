using System.Timers;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Computation;
using InformationTree.Graphics;
using D = System.Drawing;

namespace InformationTree.Extra.Graphics.Services
{
    public class GraphicsFile : IGraphicsFile, IDisposable
    {
        private readonly IGraphicsParser _graphicsParsing;

        #region Properties

        public Frame Frame { get; private set; }
        public System.Timers.Timer RunTimer { get; private set; }

        #endregion Properties

        #region Constructor

        public GraphicsFile(IGraphicsParser graphicsParsing)
        {
            _graphicsParsing = graphicsParsing;

            Frame = new Frame();
            var interval = GraphicsComputation.MillisecondsPerFrame;
            RunTimer = new System.Timers.Timer(interval);
            RunTimer.Elapsed += RunTimer_Elapsed; // timer disposed in Dispose(bool)
            RunTimer.Start();
        }

        #endregion Constructor

        #region Methods

        public void Show(D.Graphics graphics)
        {
            if (Frame != null)
                Frame.Show(graphics);
        }

        private void RunTimer_Elapsed(object? sender, ElapsedEventArgs elapsedEventArgs)
        {
            Frame.CycleFramesFromThis();
        }

        public void Clean()
        {
            Frame.Clean();
        }

        public void ParseLines(string[] lines)
        {
            _graphicsParsing.ParseLines(Frame, RunTimer, lines);
        }

        public void GoToFrame(int position)
        {
            Frame.GoToFrame(position);
        }

        public void ChangeToPreviousFrame()
        {
            Frame.ChangeToPreviousFrame();
        }

        public void ChangeToNextFrame(bool addNextFrameIfNextFrameIsNull)
        {
            Frame.ChangeToNextFrame(addNextFrameIfNextFrameIsNull);
        }

        void IDisposable.Dispose()
        {
            if (RunTimer != null)
            {
                RunTimer.Elapsed -= RunTimer_Elapsed;
                RunTimer.Enabled = false;
                RunTimer.Dispose();
            }
        }

        #endregion Methods
    }
}