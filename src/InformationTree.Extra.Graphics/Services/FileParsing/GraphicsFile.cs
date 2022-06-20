using System.Timers;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Computation;
using InformationTree.Extra.Graphics.Domain;
using InformationTree.Graphics;
using NLog;
using D = System.Drawing;

namespace InformationTree.Extra.Graphics.Services.FileParsing
{
    [Obsolete("Break into many classes later")] // TODO: 1. file parsing in one file
    public class GraphicsFile : IGraphicsFile, IDisposable
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IGraphicsFileFactory _graphicsFileFactory;
        private readonly IPopUpService _popUpService;

        #region Properties

        public Frame Frame { get; private set; }
        public System.Timers.Timer RunTimer { get; private set; }

        #endregion Properties

        #region Constructor

        public GraphicsFile(IGraphicsFileFactory graphicsFileFactory, IPopUpService popUpService)
        {
            _graphicsFileFactory = graphicsFileFactory;
            _popUpService = popUpService;

            Frame = new Frame();
            var interval = GraphicsComputation.MillisecondsPerFrame;
            RunTimer = new System.Timers.Timer(interval);
            RunTimer.Elapsed += RunTimer_Elapsed; // timer disposed in Dispose(bool)
            RunTimer.Start();
        }

        #endregion Constructor

        #region Methods

        public void ParseLines(string[] lines)
        {
            if (lines == null)
                return;
            if (lines.Length <= 0)
                return;

            var debugMessage = lines.Length <= 50 ? $"Parsing lines:{Environment.NewLine} {string.Join(Environment.NewLine, lines)}" : $"Parsing too many lines: {lines.Length} lines. Printing them to log is skipped.";
            _logger.Debug(debugMessage);

            foreach (var line in lines)
            {
                if (line.IsEmpty())
                    continue;

                var words = line.Split(' ');
                var firstWord = words[0];
                var parameters = string.Join(" ", words.Skip(1));
                bool breakFromLoop = false;

                try
                {
                    switch (firstWord)
                    {
                        case GraphicsFileConstants.FrameRelativeToPoint.DefaultName:
                        case GraphicsFileConstants.FrameRelativeToPoint.FrameRelative:
                        case GraphicsFileConstants.FrameRelativeToPoint.FrameCenter:
                            FrameRelativeToPoint(parameters);
                            break;

                        case GraphicsFileConstants.AllRelativeToPoint.DefaultName:
                        case GraphicsFileConstants.AllRelativeToPoint.RelativeToPoint:
                        case GraphicsFileConstants.AllRelativeToPoint.Relative:
                        case GraphicsFileConstants.AllRelativeToPoint.Center:
                        case GraphicsFileConstants.AllRelativeToPoint.AllRelative:
                        case GraphicsFileConstants.AllRelativeToPoint.AllCenter:
                            AllRelativeToPoint(parameters);
                            break;

                        case GraphicsFileConstants.AddFigure.DefaultName:
                        case GraphicsFileConstants.AddFigure.figure:
                        case GraphicsFileConstants.AddFigure.f:
                            AddFigure(parameters);
                            break;

                        case GraphicsFileConstants.AddText.DefaultName:
                        case GraphicsFileConstants.AddText.text:
                        case GraphicsFileConstants.AddText.txt:
                        case GraphicsFileConstants.AddText.t:
                            AddText(parameters);
                            break;

                        case GraphicsFileConstants.AddFigureOnce.DefaultName:
                        case GraphicsFileConstants.AddFigureOnce.figure_once:
                        case GraphicsFileConstants.AddFigureOnce.figureonce:
                        case GraphicsFileConstants.AddFigureOnce.fo:
                        case GraphicsFileConstants.AddFigureOnce.o:
                            AddFigureOnce(parameters);
                            break;

                        case GraphicsFileConstants.InitRotate.DefaultName:
                            InitRotate(parameters);
                            break;

                        case GraphicsFileConstants.AddRotateAround.DefaultName:
                            AddRotateAround(parameters);
                            break;

                        case GraphicsFileConstants.Rotate.DefaultName:
                            Rotate(parameters);
                            break;

                        case GraphicsFileConstants.SetPoints.DefaultName:
                        case GraphicsFileConstants.SetPoints.set_points:
                            SetPoints(parameters);
                            break;

                        case GraphicsFileConstants.SetColor.DefaultName:
                        case GraphicsFileConstants.SetColor.set_color:
                            SetColor(parameters);
                            break;

                        case GraphicsFileConstants.AddFrame.DefaultName:
                        case GraphicsFileConstants.AddFrame.add_frame:
                        case GraphicsFileConstants.AddFrame.add:
                            AddFrame();
                            break;

                        case GraphicsFileConstants.GoToFrame.DefaultName:
                        case GraphicsFileConstants.GoToFrame.goto_frame:
                        case GraphicsFileConstants.GoToFrame.GoTo:
                        case GraphicsFileConstants.GoToFrame.@goto:
                            GoToFrame(parameters);
                            break;

                        case GraphicsFileConstants.ChangeToNextFrame.DefaultName:
                        case GraphicsFileConstants.ChangeToNextFrame.next_frame:
                        case GraphicsFileConstants.ChangeToNextFrame.next:
                            ChangeToNextFrame(parameters);
                            break;

                        case GraphicsFileConstants.ChangeToPreviousFrame.DefaultName:
                        case GraphicsFileConstants.ChangeToPreviousFrame.previous_frame:
                        case GraphicsFileConstants.ChangeToPreviousFrame.prev_frame:
                        case GraphicsFileConstants.ChangeToPreviousFrame.prev:
                            ChangeToPreviousFrame();
                            break;

                        case GraphicsFileConstants.GenerateFigureLines.DefaultName:
                        case GraphicsFileConstants.GenerateFigureLines.ComputeXComputeY:
                        case GraphicsFileConstants.GenerateFigureLines.cxcy:
                        case GraphicsFileConstants.GenerateFigureLines.cpx_cpy:
                            GenerateFigureLines(parameters);
                            break;

                        case GraphicsFileConstants.Cycle.DefaultName:
                            Cycle(parameters);
                            break;

                        case GraphicsFileConstants.StopFileProcessing.DefaultName:
                        case GraphicsFileConstants.StopFileProcessing.end:
                        case GraphicsFileConstants.StopFileProcessing.x:
                        case GraphicsFileConstants.StopFileProcessing.stop:
                        case GraphicsFileConstants.StopFileProcessing.exit:
                        case GraphicsFileConstants.StopFileProcessing.q:
                        case GraphicsFileConstants.StopFileProcessing.quit:
                            breakFromLoop = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var errorWhileProcessingLine = $"Error while processing line: {line}";
                    _logger.Error(ex, errorWhileProcessingLine);
                    _popUpService.ShowError(errorWhileProcessingLine);
                }

                if (breakFromLoop)
                    break;
            }
        }

        public void Show(D.Graphics graphics)
        {
            if (Frame != null)
                Frame.Show(graphics);
        }

        #region Frame methods

        public void AddFrame()
        {
            Frame.NewFrame();
        }

        public void GoToFrame(string positionStr)
        {
            if (positionStr.IsNotEmpty()
            && int.TryParse(positionStr, out int position))
                Frame.GoToFrame(position);
        }

        public void ChangeToNextFrame(string addNextFrameIfNextFrameIsNullStr)
        {
            if (addNextFrameIfNextFrameIsNullStr.IsNotEmpty()
            && bool.TryParse(addNextFrameIfNextFrameIsNullStr, out bool addNextFrameIfNextFrameIsNull))
                Frame.ChangeToNextFrame(addNextFrameIfNextFrameIsNull);
        }

        public void ChangeToPreviousFrame()
        {
            Frame.ChangeToPreviousFrame();
        }

        public void CleanCurrentFrame()
        {
            Frame.CleanThisFrame();
        }

        public void CleanAllFrames()
        {
            Frame.CleanAllFrames();
        }

        public void Clean()
        {
            Frame.Clean();
        }

        public void Cycle(string intervalStr)
        {
            if (intervalStr.IsEmpty())
                return;

            if (int.TryParse(intervalStr, out int interval))
            {
                try
                {
                    // Start timer
                    RunTimer.Enabled = false;
                    RunTimer.Interval = interval;
                    RunTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    // Stop timer
                    RunTimer.Enabled = false;

                    _logger.Error(ex, $"{nameof(GraphicsFile)}.{nameof(Cycle)} parsing '{intervalStr}' issue.");
                    _popUpService.ShowError(ex.Message, $"{nameof(FrameRelativeToPoint)} Error");
                }
            }
        }

        public void FrameRelativeToPoint(string parameters)
        {
            if (parameters.IsEmpty())
                return;
            try
            {
                var words = parameters.Split(' ');

                switch (words.Length)
                {
                    case 4:
                        if (!int.TryParse(words[0], out var oldX))
                            throw new Exception($"{nameof(GraphicsFile)}.{nameof(FrameRelativeToPoint)} parsing '{words[0]}' issue as {typeof(int).Name}.");
                        if (!int.TryParse(words[1], out var oldY))
                            throw new Exception($"{nameof(GraphicsFile)}.{nameof(FrameRelativeToPoint)} parsing '{words[1]}' issue as {typeof(int).Name}.");
                        if (!int.TryParse(words[2], out var newX))
                            throw new Exception($"{nameof(GraphicsFile)}.{nameof(FrameRelativeToPoint)} parsing '{words[2]}' issue as {typeof(int).Name}.");
                        if (!int.TryParse(words[3], out var newY))
                            throw new Exception($"{nameof(GraphicsFile)}.{nameof(FrameRelativeToPoint)} parsing '{words[3]}' issue as {typeof(int).Name}.");

                        var oldPoint = new Point
                        {
                            X = oldX,
                            Y = oldY
                        };
                        var newPoint = new Point
                        {
                            X = newX,
                            Y = newY
                        };

                        Frame.TranslateCenter(oldPoint, newPoint);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(GraphicsFile)}.{nameof(FrameRelativeToPoint)}: '{parameters}' splitting and parsing issue: {ex.Message}.");
                _popUpService.ShowError(ex.Message, $"{nameof(FrameRelativeToPoint)} Error");
            }
        }

        private void AllRelativeToPoint(string parameters)
        {
            if (parameters.IsEmpty())
                return;
            try
            {
                var words = parameters.Split(' ');

                switch (words.Length)
                {
                    case 4:
                        if (!int.TryParse(words[0], out var oldX))
                            throw new Exception($"{nameof(GraphicsFile)}.{nameof(AllRelativeToPoint)} parsing '{words[0]}' issue as {typeof(int).Name}.");
                        if (!int.TryParse(words[1], out var oldY))
                            throw new Exception($"{nameof(GraphicsFile)}.{nameof(AllRelativeToPoint)} parsing '{words[1]}' issue as {typeof(int).Name}.");
                        if (!int.TryParse(words[2], out var newX))
                            throw new Exception($"{nameof(GraphicsFile)}.{nameof(AllRelativeToPoint)} parsing '{words[2]}' issue as {typeof(int).Name}.");
                        if (!int.TryParse(words[3], out var newY))
                            throw new Exception($"{nameof(GraphicsFile)}.{nameof(AllRelativeToPoint)} parsing '{words[3]}' issue as {typeof(int).Name}.");

                        var oldPoint = new Point
                        {
                            X = oldX,
                            Y = oldY
                        };
                        var newPoint = new Point
                        {
                            X = newX,
                            Y = newY
                        };

                        Frame.TranslateCenterAllFrames(oldPoint, newPoint);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(GraphicsFile)}.{nameof(AllRelativeToPoint)}: '{parameters}' splitting and parsing issue: {ex.Message}.");
                _popUpService.ShowError($"Error '{ex.Message}' occured while parsing '{parameters}' in {nameof(AllRelativeToPoint)} method");
            }
        }

        private void RunTimer_Elapsed(object? sender, ElapsedEventArgs elapsedEventArgs)
        {
            CycleFrames();
        }

        private void CycleFrames()
        {
            Frame.CycleFramesFromThis();
        }

        #endregion Frame methods

        #region Graphics generator methods

        public void GenerateFigureLines(string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var words = parameters.Split(' ');
            double _radius;
            int _iterations;
            ComputeType _computeType;

            switch (words.Length)
            {
                case 2:
                case 3:
                    if(!double.TryParse(words[0], out _radius))
                        throw new Exception($"{nameof(GraphicsFile)}.{nameof(GenerateFigureLines)}: '{words[0]}' parsing issue as {typeof(double).Name}.");
                    if (!int.TryParse(words[1], out _iterations))
                        throw new Exception($"{nameof(GraphicsFile)}.{nameof(GenerateFigureLines)}: '{words[1]}' parsing issue as {typeof(int).Name}.");
                    if (words.Length == 3
                        && !Enum.TryParse(words[2], out _computeType))
                        throw new Exception($"{nameof(GraphicsFile)}.{nameof(GenerateFigureLines)}: '{words[2]}' parsing issue as {nameof(ComputeType)}.");
                    else // Words length is 2
                        _computeType = ComputeType.ExtraFiguresWithPointsNumberOfCorners;
                    
                    ParseLines(_graphicsFileFactory.GenerateFigureLines(_radius, _iterations, _computeType).Distinct().ToArray());
                    break;

                case 7:
                case 8:
                    if (!int.TryParse(words[0], out var _points))
                        throw new Exception($"{nameof(GraphicsFile)}.{nameof(GenerateFigureLines)}: '{words[0]}' parsing issue as {typeof(int).Name}.");
                    if (!double.TryParse(words[1], out var _x))
                        throw new Exception($"{nameof(GraphicsFile)}.{nameof(GenerateFigureLines)}: '{words[1]}' parsing issue as {typeof(double).Name}.");
                    if (!double.TryParse(words[2], out var _y))
                        throw new Exception($"{nameof(GraphicsFile)}.{nameof(GenerateFigureLines)}: '{words[2]}' parsing issue as {typeof(double).Name}.");
                    if (!double.TryParse(words[3], out _radius))
                        throw new Exception($"{nameof(GraphicsFile)}.{nameof(GenerateFigureLines)}: '{words[3]}' parsing issue as {typeof(double).Name}.");
                    if (!double.TryParse(words[4], out var _theta))
                        throw new Exception($"{nameof(GraphicsFile)}.{nameof(GenerateFigureLines)}: '{words[4]}' parsing issue as {typeof(double).Name}.");
                    if (!int.TryParse(words[5], out var _number))
                        throw new Exception($"{nameof(GraphicsFile)}.{nameof(GenerateFigureLines)}: '{words[5]}' parsing issue as {typeof(int).Name}.");
                    if (!int.TryParse(words[6], out _iterations))
                        throw new Exception($"{nameof(GraphicsFile)}.{nameof(GenerateFigureLines)}: '{words[6]}' parsing issue as {typeof(int).Name}.");
                    if (words.Length == 8
                        && !Enum.TryParse(words[7], out _computeType))
                        throw new Exception($"{nameof(GraphicsFile)}.{nameof(GenerateFigureLines)}: '{words[7]}' parsing issue as {nameof(ComputeType)}.");
                    else // Words length is 7
                        _computeType = ComputeType.ExtraFiguresWithPointsNumberOfCorners;
                    
                    ParseLines(_graphicsFileFactory.GenerateFigureLines(_points, _x, _y, _radius, _theta, _number, _iterations, _computeType).Distinct().ToArray());
                    break;
            }
        }

        #endregion Graphics generator methods

        #region Add figure methods

        public void AddFigure(string figureLine)
        {
            if (figureLine.IsEmpty())
                return;
            var figure = FigureFactory.GetFigure(figureLine);
            if (figure == null)
                return;
            Frame.GetActiveFrameOrThis().Figures.AddFigure(figure);
        }

        public void AddText(string textLine)
        {
            if (textLine.IsEmpty())
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.AddText(textLine);
        }

        public void AddFigureOnce(string figureLine)
        {
            if (figureLine.IsEmpty())
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.AddFigureOnce(FigureFactory.GetFigure(figureLine));
        }

        #endregion Add figure methods

        #region Config figure methods

        public void InitRotate(string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.InitRotate(parameters);
        }

        public void Rotate(string positionStr)
        {
            if (positionStr.IsEmpty())
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.Rotate(positionStr);
        }

        public void AddRotateAround(string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.AddRotateAround(parameters);
        }

        public void SetPoints(string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.SetPoints(parameters);
        }

        public void SetColor(string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.SetColor(parameters);
        }

        #endregion Config figure methods

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