using InformationTree.Extra.Graphics.Computation;
using InformationTree.Graphics;
using D = System.Drawing;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Domain;
using InformationTree.Domain.Entities.Graphics;
using System.Timers;
using NLog;
using InformationTree.Domain.Services;

namespace InformationTree.Extra.Graphics.Services.FileParsing
{
    [Obsolete("Break into many classes later")] // TODO: file parsing in one file, figures drawing in another
    public class GraphicsFile : IGraphicsFile, IDisposable
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IGraphicsFileFactory _graphicsFileRecursiveGenerator;
        private readonly IPopUpService _popUpService;

        #region Properties

        public Frame Frame { get; private set; }
        public System.Timers.Timer RunTimer { get; set; }

        #endregion Properties

        #region Constructor

        public GraphicsFile(IGraphicsFileFactory graphicsProvider, IPopUpService popUpService)
        {
            _graphicsFileRecursiveGenerator = graphicsProvider;
            _popUpService = popUpService;

            Frame = new Frame();
            var interval = GraphicsComputation.MillisecondsPerFrame;
            RunTimer = new System.Timers.Timer(interval);
            RunTimer.Elapsed += RunTimer_Elapsed; // timer disposed in Dispose(bool)
            RunTimer.Start();
        }

        #endregion Constructor

        #region Methods

        public void ParseFile(string fileName)
        {
            ParseLines(File.ReadAllLines(fileName));
        }

        public void ParseLines(string[] lines)
        {
            if (lines.Length <= 0)
                return;

            var debugMessage = lines.Length <= 50 ? $"Parsing lines: {lines}" : $"Parsing too many lines: {lines.Length} lines. Printing them to log is skipped.";
            _logger.Debug(debugMessage);
            
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
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

        public void ParseLines(string lines)
        {
            ParseLines(lines.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries));
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
            if (string.IsNullOrEmpty(positionStr))
                return;
            Frame.GoToFrame(int.Parse(positionStr));
        }

        public void ChangeToNextFrame(string addNextFrameIfNextFrameIsNullStr)
        {
            if (string.IsNullOrEmpty(addNextFrameIfNextFrameIsNullStr))
                return;
            Frame.ChangeToNextFrame(bool.Parse(addNextFrameIfNextFrameIsNullStr));
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
            if (RunTimer == null)
                return;
            if (string.IsNullOrEmpty(intervalStr))
                return;

            try
            {
                // Start timer
                RunTimer.Enabled = false;
                var interval = int.Parse(intervalStr);
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

        public void FrameRelativeToPoint(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
                return;
            try
            {
                var words = parameters.Split(' ');

                switch (words.Length)
                {
                    case 4:
                        var oldPoint = new Point
                        {
                            X = int.Parse(words[0]),
                            Y = int.Parse(words[1])
                        };
                        var newPoint = new Point
                        {
                            X = int.Parse(words[2]),
                            Y = int.Parse(words[3])
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
            if (string.IsNullOrEmpty(parameters))
                return;
            try
            {
                var words = parameters.Split(' ');

                switch (words.Length)
                {
                    case 4:
                        var oldPoint = new Point
                        {
                            X = int.Parse(words[0]),
                            Y = int.Parse(words[1])
                        };
                        var newPoint = new Point
                        {
                            X = int.Parse(words[2]),
                            Y = int.Parse(words[3])
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
            if (string.IsNullOrEmpty(parameters))
                return;
            var words = parameters.Split(' ');
            double _radius;
            int _iterations;
            ComputeType _computeType;

            switch (words.Length)
            {
                case 2:
                case 3:
                    _radius = double.Parse(words[0]);
                    _iterations = int.Parse(words[1]);
                    _computeType = words.Length == 3 ? (ComputeType)int.Parse(words[2]) : ComputeType.ExtraFiguresWithPointsNumberOfCorners;
                    ParseLines(_graphicsFileRecursiveGenerator.GenerateFigureLines(_radius, _iterations, _computeType).Distinct().ToArray());
                    break;

                case 7:
                case 8:
                    var _points = int.Parse(words[0]);
                    var _x = double.Parse(words[1]);
                    var _y = double.Parse(words[2]);
                    _radius = double.Parse(words[3]);
                    var _theta = double.Parse(words[4]);
                    var _number = int.Parse(words[5]);
                    _iterations = int.Parse(words[6]);
                    _computeType = words.Length == 8 ? (ComputeType)int.Parse(words[7]) : ComputeType.ExtraFiguresWithPointsNumberOfCorners;
                    ParseLines(_graphicsFileRecursiveGenerator.GenerateFigureLines(_points, _x, _y, _radius, _theta, _number, _iterations, _computeType).Distinct().ToArray());
                    break;
            }
        }

        #endregion Graphics generator methods

        #region Add figure methods

        public void AddFigure(string figureLine)
        {
            if (string.IsNullOrEmpty(figureLine))
                return;
            var figure = FigureFactory.GetFigure(figureLine);
            if (figure == null)
                return;
            Frame.GetActiveFrameOrThis().Figures.AddFigure(figure);
        }

        public void AddText(string textLine)
        {
            if (string.IsNullOrEmpty(textLine))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.AddText(textLine);
        }

        public void AddFigureOnce(string figureLine)
        {
            if (string.IsNullOrEmpty(figureLine))
                return;
            if (Frame == null)
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
            if (string.IsNullOrEmpty(parameters))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.InitRotate(parameters);
        }

        public void Rotate(string positionStr)
        {
            if (string.IsNullOrEmpty(positionStr))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.Rotate(positionStr);
        }

        public void AddRotateAround(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.AddRotateAround(parameters);
        }

        public void SetPoints(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.SetPoints(parameters);
        }

        public void SetColor(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
                return;
            if (Frame == null)
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