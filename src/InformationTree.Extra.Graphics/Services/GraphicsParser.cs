using InformationTree.Domain;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Domain;
using NLog;

namespace InformationTree.Extra.Graphics.Services
{
    public class GraphicsParser : IGraphicsParser
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IGraphicsFileFactory _graphicsFileFactory;
        private readonly IPopUpService _popUpService;

        public GraphicsParser(
            IGraphicsFileFactory graphicsFileFactory,
            IPopUpService popUpService)
        {
            _graphicsFileFactory = graphicsFileFactory;
            _popUpService = popUpService;
        }

        public void ParseLines(IFrame frame, System.Timers.Timer runTimer, string[] lines)
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

                var words = line.Split(Constants.Parsing.SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                var firstWord = words[0];
                var parameters = string.Join(Constants.Parsing.SpaceSeparator, words.Skip(1));
                bool breakFromLoop = false;

                try
                {
                    switch (firstWord)
                    {
                        case GraphicsFileConstants.FrameRelativeToPoint.DefaultName:
                        case GraphicsFileConstants.FrameRelativeToPoint.FrameRelative:
                        case GraphicsFileConstants.FrameRelativeToPoint.FrameCenter:
                            FrameRelativeToPoint(frame, parameters);
                            break;

                        case GraphicsFileConstants.AllRelativeToPoint.DefaultName:
                        case GraphicsFileConstants.AllRelativeToPoint.RelativeToPoint:
                        case GraphicsFileConstants.AllRelativeToPoint.Relative:
                        case GraphicsFileConstants.AllRelativeToPoint.Center:
                        case GraphicsFileConstants.AllRelativeToPoint.AllRelative:
                        case GraphicsFileConstants.AllRelativeToPoint.AllCenter:
                            AllRelativeToPoint(frame, parameters);
                            break;

                        case GraphicsFileConstants.AddFigure.DefaultName:
                        case GraphicsFileConstants.AddFigure.figure:
                        case GraphicsFileConstants.AddFigure.f:
                            AddFigure(frame, parameters);
                            break;

                        case GraphicsFileConstants.AddText.DefaultName:
                        case GraphicsFileConstants.AddText.text:
                        case GraphicsFileConstants.AddText.txt:
                        case GraphicsFileConstants.AddText.t:
                            AddText(frame, parameters);
                            break;

                        case GraphicsFileConstants.AddFigureOnce.DefaultName:
                        case GraphicsFileConstants.AddFigureOnce.figure_once:
                        case GraphicsFileConstants.AddFigureOnce.figureonce:
                        case GraphicsFileConstants.AddFigureOnce.fo:
                        case GraphicsFileConstants.AddFigureOnce.o:
                            AddFigureOnce(frame, parameters);
                            break;

                        case GraphicsFileConstants.InitRotate.DefaultName:
                            InitRotate(frame, parameters);
                            break;

                        case GraphicsFileConstants.AddRotateAround.DefaultName:
                            AddRotateAround(frame, parameters);
                            break;

                        case GraphicsFileConstants.Rotate.DefaultName:
                            Rotate(frame, parameters);
                            break;

                        case GraphicsFileConstants.SetPoints.DefaultName:
                        case GraphicsFileConstants.SetPoints.set_points:
                            SetPoints(frame, parameters);
                            break;

                        case GraphicsFileConstants.SetColor.DefaultName:
                        case GraphicsFileConstants.SetColor.set_color:
                            SetColor(frame, parameters);
                            break;

                        case GraphicsFileConstants.AddFrame.DefaultName:
                        case GraphicsFileConstants.AddFrame.add_frame:
                        case GraphicsFileConstants.AddFrame.add:
                            frame.NewFrame();
                            break;

                        case GraphicsFileConstants.GoToFrame.DefaultName:
                        case GraphicsFileConstants.GoToFrame.goto_frame:
                        case GraphicsFileConstants.GoToFrame.GoTo:
                        case GraphicsFileConstants.GoToFrame.@goto:
                            GoToFrame(frame, parameters);
                            break;

                        case GraphicsFileConstants.ChangeToNextFrame.DefaultName:
                        case GraphicsFileConstants.ChangeToNextFrame.next_frame:
                        case GraphicsFileConstants.ChangeToNextFrame.next:
                            ChangeToNextFrame(frame, parameters);
                            break;

                        case GraphicsFileConstants.ChangeToPreviousFrame.DefaultName:
                        case GraphicsFileConstants.ChangeToPreviousFrame.previous_frame:
                        case GraphicsFileConstants.ChangeToPreviousFrame.prev_frame:
                        case GraphicsFileConstants.ChangeToPreviousFrame.prev:
                            frame.ChangeToPreviousFrame();
                            break;

                        case GraphicsFileConstants.GenerateFigureLines.DefaultName:
                        case GraphicsFileConstants.GenerateFigureLines.ComputeXComputeY:
                        case GraphicsFileConstants.GenerateFigureLines.cxcy:
                        case GraphicsFileConstants.GenerateFigureLines.cpx_cpy:
                            GenerateFigureLines(frame, runTimer, parameters);
                            break;

                        case GraphicsFileConstants.Cycle.DefaultName:
                            Cycle(frame, runTimer, parameters);
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

        public void FrameRelativeToPoint(IFrame frame, string parameters)
        {
            if (parameters.IsEmpty())
                return;
            try
            {
                var words = parameters.Split(Constants.Parsing.SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);

                switch (words.Length)
                {
                    case 4:
                        if (!int.TryParse(words[0], out var oldX))
                            throw new Exception($"{nameof(GraphicsParser)}.{nameof(FrameRelativeToPoint)} parsing '{words[0]}' issue as {typeof(int).Name}.");
                        if (!int.TryParse(words[1], out var oldY))
                            throw new Exception($"{nameof(GraphicsParser)}.{nameof(FrameRelativeToPoint)} parsing '{words[1]}' issue as {typeof(int).Name}.");
                        if (!int.TryParse(words[2], out var newX))
                            throw new Exception($"{nameof(GraphicsParser)}.{nameof(FrameRelativeToPoint)} parsing '{words[2]}' issue as {typeof(int).Name}.");
                        if (!int.TryParse(words[3], out var newY))
                            throw new Exception($"{nameof(GraphicsParser)}.{nameof(FrameRelativeToPoint)} parsing '{words[3]}' issue as {typeof(int).Name}.");

                        var oldCenter = new Point
                        {
                            X = oldX,
                            Y = oldY
                        };
                        var newCenter = new Point
                        {
                            X = newX,
                            Y = newY
                        };

                        frame.TranslateCenter(oldCenter, newCenter);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(GraphicsParser)}.{nameof(FrameRelativeToPoint)}: '{parameters}' splitting and parsing issue: {ex.Message}.");
                _popUpService.ShowError(ex.Message, $"{nameof(FrameRelativeToPoint)} Error");
            }
        }

        private void AllRelativeToPoint(IFrame frame, string parameters)
        {
            if (parameters.IsEmpty())
                return;
            try
            {
                var words = parameters.Split(Constants.Parsing.SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);

                switch (words.Length)
                {
                    case 4:
                        if (!int.TryParse(words[0], out var oldX))
                            throw new Exception($"{nameof(GraphicsParser)}.{nameof(AllRelativeToPoint)} parsing '{words[0]}' issue as {typeof(int).Name}.");
                        if (!int.TryParse(words[1], out var oldY))
                            throw new Exception($"{nameof(GraphicsParser)}.{nameof(AllRelativeToPoint)} parsing '{words[1]}' issue as {typeof(int).Name}.");
                        if (!int.TryParse(words[2], out var newX))
                            throw new Exception($"{nameof(GraphicsParser)}.{nameof(AllRelativeToPoint)} parsing '{words[2]}' issue as {typeof(int).Name}.");
                        if (!int.TryParse(words[3], out var newY))
                            throw new Exception($"{nameof(GraphicsParser)}.{nameof(AllRelativeToPoint)} parsing '{words[3]}' issue as {typeof(int).Name}.");

                        var oldCenter = new Point
                        {
                            X = oldX,
                            Y = oldY
                        };
                        var newCenter = new Point
                        {
                            X = newX,
                            Y = newY
                        };

                        frame.TranslateCenterAllFrames(oldCenter, newCenter);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(GraphicsParser)}.{nameof(AllRelativeToPoint)}: '{parameters}' splitting and parsing issue: {ex.Message}.");
                _popUpService.ShowError($"Error '{ex.Message}' occured while parsing '{parameters}' in {nameof(AllRelativeToPoint)} method");
            }
        }

        private void AddFigure(IFrame frame, string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var figure = FigureFactory.GetFigure(parameters);
            if (figure == null)
                return;
            frame.GetActiveFrameOrThis().Figures.AddFigure(figure);
        }

        private void AddText(IFrame frame, string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var figures = frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.AddText(parameters);
        }

        private void AddFigureOnce(IFrame frame, string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var figures = frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.AddFigureOnce(FigureFactory.GetFigure(parameters));
        }

        private void InitRotate(IFrame frame, string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var figures = frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.InitRotate(parameters);
        }

        private void AddRotateAround(IFrame frame, string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var figures = frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.AddRotateAround(parameters);
        }

        private void Rotate(IFrame frame, string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var figures = frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.Rotate(parameters);
        }

        private void SetPoints(IFrame frame, string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var figures = frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.SetPoints(parameters);
        }

        private void SetColor(IFrame frame, string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var figures = frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.SetColor(parameters);
        }

        private void GoToFrame(IFrame frame, string parameters)
        {
            if (parameters.IsNotEmpty()
            && int.TryParse(parameters, out int position))
                frame.GoToFrame(position);
        }

        private void ChangeToNextFrame(IFrame frame, string parameters)
        {
            if (parameters.IsNotEmpty()
            && bool.TryParse(parameters, out bool addNextFrameIfNextFrameIsNull))
                frame.ChangeToNextFrame(addNextFrameIfNextFrameIsNull);
        }

        private void GenerateFigureLines(IFrame frame, System.Timers.Timer runTimer, string parameters)
        {
            if (parameters.IsEmpty())
                return;
            var words = parameters.Split(Constants.Parsing.SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
            double _radius;
            int _iterations;
            ComputeType _computeType;

            switch (words.Length)
            {
                case 2:
                case 3:
                    if (!double.TryParse(words[0], out _radius))
                        throw new Exception($"{nameof(GraphicsParser)}.{nameof(GenerateFigureLines)}: '{words[0]}' parsing issue as {typeof(double).Name}.");
                    if (!int.TryParse(words[1], out _iterations))
                        throw new Exception($"{nameof(GraphicsParser)}.{nameof(GenerateFigureLines)}: '{words[1]}' parsing issue as {typeof(int).Name}.");
                    if (words.Length == 3
                        && !Enum.TryParse(words[2], out _computeType))
                        throw new Exception($"{nameof(GraphicsParser)}.{nameof(GenerateFigureLines)}: '{words[2]}' parsing issue as {nameof(ComputeType)}.");
                    else // Words length is 2
                        _computeType = ComputeType.ExtraFiguresWithPointsNumberOfCorners;

                    ParseLines(frame, runTimer, _graphicsFileFactory.GenerateFigureLines(_radius, _iterations, _computeType).Distinct().ToArray());
                    break;

                case 7:
                case 8:
                    if (!int.TryParse(words[0], out var _points))
                        throw new Exception($"{nameof(GraphicsParser)}.{nameof(GenerateFigureLines)}: '{words[0]}' parsing issue as {typeof(int).Name}.");
                    if (!double.TryParse(words[1], out var _x))
                        throw new Exception($"{nameof(GraphicsParser)}.{nameof(GenerateFigureLines)}: '{words[1]}' parsing issue as {typeof(double).Name}.");
                    if (!double.TryParse(words[2], out var _y))
                        throw new Exception($"{nameof(GraphicsParser)}.{nameof(GenerateFigureLines)}: '{words[2]}' parsing issue as {typeof(double).Name}.");
                    if (!double.TryParse(words[3], out _radius))
                        throw new Exception($"{nameof(GraphicsParser)}.{nameof(GenerateFigureLines)}: '{words[3]}' parsing issue as {typeof(double).Name}.");
                    if (!double.TryParse(words[4], out var _theta))
                        throw new Exception($"{nameof(GraphicsParser)}.{nameof(GenerateFigureLines)}: '{words[4]}' parsing issue as {typeof(double).Name}.");
                    if (!int.TryParse(words[5], out var _number))
                        throw new Exception($"{nameof(GraphicsParser)}.{nameof(GenerateFigureLines)}: '{words[5]}' parsing issue as {typeof(int).Name}.");
                    if (!int.TryParse(words[6], out _iterations))
                        throw new Exception($"{nameof(GraphicsParser)}.{nameof(GenerateFigureLines)}: '{words[6]}' parsing issue as {typeof(int).Name}.");
                    if (words.Length == 8
                        && !Enum.TryParse(words[7], out _computeType))
                        throw new Exception($"{nameof(GraphicsParser)}.{nameof(GenerateFigureLines)}: '{words[7]}' parsing issue as {nameof(ComputeType)}.");
                    else // Words length is 7
                        _computeType = ComputeType.ExtraFiguresWithPointsNumberOfCorners;

                    ParseLines(frame, runTimer, _graphicsFileFactory.GenerateFigureLines(_points, _x, _y, _radius, _theta, _number, _iterations, _computeType).Distinct().ToArray());
                    break;
            }
        }

        private void Cycle(IFrame frame, System.Timers.Timer runTimer, string parameters)
        {
            if (parameters.IsEmpty())
                return;

            if (int.TryParse(parameters, out int interval))
            {
                try
                {
                    // Start timer
                    runTimer.Enabled = false;
                    runTimer.Interval = interval;
                    runTimer.Enabled = true;
                }
                catch (Exception ex)
                {
                    // Stop timer
                    runTimer.Enabled = false;

                    _logger.Error(ex, $"{nameof(GraphicsParser)}.{nameof(Cycle)} parsing '{parameters}' issue.");
                    _popUpService.ShowError(ex.Message, $"{nameof(FrameRelativeToPoint)} Error");
                }
            }
        }
    }
}