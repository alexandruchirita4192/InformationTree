using System.Text;
using InformationTree.Extra.Graphics.Computation;
using InformationTree.Graphics;
using D = System.Drawing;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Domain;
using InformationTree.Domain.Entities.Graphics;

namespace InformationTree.Extra.Graphics.Services.FileParsing
{
    [Obsolete("Break into many classes later")] // TODO: file parsing in one file, figures drawing in another
    public class GraphicsFile : IDisposable
    {
        private IGraphicsFileRecursiveGenerator _graphicsProvider;

        #region Properties

        public Frame Frame { get; private set; }
        public System.Threading.Timer RunTimer { get; set; }

        #endregion Properties

        #region Constructor

        public GraphicsFile(IGraphicsFileRecursiveGenerator graphicsProvider)
        {
            _graphicsProvider = graphicsProvider;

            Frame = new Frame();
            var interval = GraphicsComputation.MillisecondsPerFrame;
            RunTimer = new System.Threading.Timer(RunTimer_Tick, null, interval, interval);
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

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                //MessageBox.Show(GetDebugText());

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

                        case GraphicsFileConstants.ComputeXComputeY.DefaultName:
                        case GraphicsFileConstants.ComputeXComputeY.cxcy:
                        case GraphicsFileConstants.ComputeXComputeY.cpx_cpy:
                            ComputeXComputeY(parameters);
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
                    // TODO: Create a logger or a UI error issue show (error handling)
                    //MessageBox.Show("firstWord: " + firstWord + ";parameters=" + parameters + ";" + Environment.NewLine + ex.ToString());
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

        public void GoToFrame(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            Frame.GoToFrame(int.Parse(s));
        }

        public void ChangeToNextFrame(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            Frame.ChangeToNextFrame(bool.Parse(s));
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

        public void Cycle(string s)
        {
            if (RunTimer == null)
                return;
            if (string.IsNullOrEmpty(s))
                return;

            try
            {
                // Start timer
                var interval = int.Parse(s);
                RunTimer.Change(interval, interval);
            }
            catch
            {
                // Stop timer
                try { RunTimer.Change(Timeout.Infinite, Timeout.Infinite); } catch { /* silent crash */ }
            }
        }

        // TODO: Remove showing issues to UI directly
        public void FrameRelativeToPoint(string parameters)
        {
            try
            {
                var words = parameters.Split(' ');
                D.Point oldPoint = new D.Point(), newPoint = new D.Point();

                switch (words.Length)
                {
                    case 4:
                        oldPoint.X = int.Parse(words[0]);
                        oldPoint.Y = int.Parse(words[1]);
                        newPoint.X = int.Parse(words[2]);
                        newPoint.Y = int.Parse(words[3]);
                        Frame.TranslateCenter(oldPoint, newPoint);
                        break;
                }
            }
            catch (Exception ex)
            {
                // TODO: Create a logger or a UI error issue show (error handling)
                //MessageBox.Show("parameters=" + parameters + ";" + Environment.NewLine + ex.ToString());
            }
        }

        private void AllRelativeToPoint(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
                return;
            try
            {
                var words = parameters.Split(' ');
                var oldPoint = new Point();
                var newPoint = new Point();

                switch (words.Length)
                {
                    case 4:
                        oldPoint.X = int.Parse(words[0]);
                        oldPoint.Y = int.Parse(words[1]);
                        newPoint.X = int.Parse(words[2]);
                        newPoint.Y = int.Parse(words[3]);
                        Frame.TranslateCenterAllFrames(oldPoint, newPoint);
                        break;
                }
            }
            catch (Exception ex)
            {
                // TODO: Create a logger or a UI error issue show (error handling)
                //MessageBox.Show("parameters=" + parameters + ";" + Environment.NewLine + ex.ToString());
            }
        }

        private void RunTimer_Tick(object? sender)
        {
            CycleFrames();
        }

        private void CycleFrames()
        {
            Frame.CycleFramesFromThis();
        }

        #endregion Frame methods

        #region Graphics generator methods

        public void ComputeXComputeY(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            var words = s.Split(' ');
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
                    ParseLines(_graphicsProvider.GenerateFigureLines(_radius, _iterations, _computeType).Distinct().ToArray());
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
                    ParseLines(_graphicsProvider.GenerateFigureLines(_points, _x, _y, _radius, _theta, _number, _iterations, _computeType).Distinct().ToArray());
                    break;
            }
        }

        #endregion Graphics generator methods

        #region Add figure methods

        public void AddFigure(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            Frame.GetActiveFrameOrThis().Figures.AddFigure(FigureFactory.GetFigure(s));
        }

        public void AddText(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.AddText(s);
        }

        public void AddFigureOnce(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.AddFigureOnce(FigureFactory.GetFigure(s));
        }

        #endregion Add figure methods

        #region Config figure methods

        public void InitRotate(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.InitRotate(s);
        }

        public void Rotate(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.Rotate(s);
        }

        public void AddRotateAround(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.AddRotateAround(s);
        }

        public void SetPoints(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.SetPoints(s);
        }

        public void SetColor(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.SetColor(s);
        }

        #endregion Config figure methods

        #region Debug methods

        public string GetDebugText()
        {
            var stringBuilder = new StringBuilder();
            foreach (var f in Frame.Figures.FigureList)
                stringBuilder.AppendLine("figure(" + f.GetDebugText() + ");");
            return stringBuilder.ToString();
        }

        #endregion Debug methods

        void IDisposable.Dispose()
        {
            if (RunTimer != null)
            {
                RunTimer.Change(Timeout.Infinite, Timeout.Infinite);
                RunTimer.Dispose();
            }
        }

        #endregion Methods
    }
}