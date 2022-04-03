using System.Text;
using InformationTree.Extra.Graphics.Computation;
using InformationTree.Graphics;
using D = System.Drawing;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Domain;

namespace InformationTree.Extra.Graphics.Services.FileParsing
{
    [Obsolete("Break into many classes later")] // TODO: file parsing in one file, figures drawing in another
    public class GraphicsFile : IDisposable
    {
        /// <summary>
        /// Constants class with all commands (including some legacy commands)
        /// </summary>
        public static class Constants
        {
            public static class FrameRelativeToPoint
            {
                public const string DefaultName = $"{nameof(FrameRelativeToPoint)}";
                public const string FrameRelative = "FrameRelative";
                public const string FrameCenter = "FrameCenter";
            }
            
            public static class AllRelativeToPoint
            {
                public const string DefaultName = $"{nameof(AllRelativeToPoint)}";
                public const string RelativeToPoint = "RelativeToPoint";
                public const string Relative = "Relative";
                public const string Center = "Center";
                public const string AllRelative = "AllRelative";
                public const string AllCenter = "AllCenter";
            }
            
            public static class AddFigure
            {
                public const string DefaultName = $"{nameof(AddFigure)}";
                public const string f = "f";
                public const string figure = "figure";
            }

            public static class AddText
            {
                public const string DefaultName = $"{nameof(AddText)}";
                public const string t = "t";
                public const string txt = "txt";
                public const string text = "text";
            }

            public static class AddFigureOnce
            {
                public const string DefaultName = $"{nameof(AddFigureOnce)}";
                public const string o = "o";
                public const string fo = "fo";
                public const string figureonce = "figureonce";
                public const string figure_once = "figure_once";
            }

            public static class InitRotate
            {
                public const string DefaultName = $"{nameof(InitRotate)}";
            }
            public static class AddRotateAround
            {
                public const string DefaultName = $"{nameof(AddRotateAround)}";
            }
            public static class Rotate
            {
                public const string DefaultName = $"{nameof(Rotate)}";
            }

            public static class SetPoints
            {
                public const string DefaultName = $"{nameof(SetPoints)}";
                public const string set_points = "set_points";
            }
            public static class SetColor
            {
                public const string DefaultName = $"{nameof(SetColor)}";
                public const string set_color = "set_color";
            }

            public static class AddFrame
            {
                public const string DefaultName = $"{nameof(AddFrame)}";
                public const string add_frame = "add_frame";
                public const string add = "add";
            }
            public static class GoToFrame
            {
                public const string DefaultName = $"{nameof(GoToFrame)}";
                public const string goto_frame = "goto_frame";
                public const string GoTo = "GoTo";
                public const string @goto = "goto";
            }
            public static class ChangeToNextFrame
            {
                public const string DefaultName = $"{nameof(ChangeToNextFrame)}";
                public const string next_frame = "next_frame";
                public const string next = "next";
            }
            public static class ChangeToPreviousFrame
            {
                public const string DefaultName = $"{nameof(ChangeToPreviousFrame)}";
                public const string previous_frame = "previous_frame";
                public const string prev_frame = "prev_frame";
                public const string prev = "prev";
            }

            // TODO: Rename this to explain that this recursively generates a lot of figures
            public static class ComputeXComputeY
            {
                public const string DefaultName = $"{nameof(ComputeXComputeY)}";
                public const string cxcy = "cxcy";
                public const string cpx_cpy = "cpx_cpy";
            }
            public static class Cycle
            {
                public const string DefaultName = $"{nameof(Cycle)}";
            }
            public static class StopFileProcessing
            {
                public const string DefaultName = $"{nameof(StopFileProcessing)}";
                public const string end = "end";
                public const string x = "x";
                public const string stop = "stop";
                public const string exit = "exit";
                public const string q = "q";
                public const string quit = "quit";
            }
        }

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
                        case Constants.FrameRelativeToPoint.DefaultName:
                        case Constants.FrameRelativeToPoint.FrameRelative:
                        case Constants.FrameRelativeToPoint.FrameCenter:
                            FrameRelativeToPoint(parameters);
                            break;

                        case Constants.AllRelativeToPoint.DefaultName:
                        case Constants.AllRelativeToPoint.RelativeToPoint:
                        case Constants.AllRelativeToPoint.Relative:
                        case Constants.AllRelativeToPoint.Center:
                        case Constants.AllRelativeToPoint.AllRelative:
                        case Constants.AllRelativeToPoint.AllCenter:
                            AllRelativeToPoint(parameters);
                            break;

                        case Constants.AddFigure.DefaultName:
                        case Constants.AddFigure.figure:
                        case Constants.AddFigure.f:
                            AddFigure(parameters);
                            break;

                        case Constants.AddText.DefaultName:
                        case Constants.AddText.text:
                        case Constants.AddText.txt:
                        case Constants.AddText.t:
                            AddText(parameters);
                            break;

                        case Constants.AddFigureOnce.DefaultName:
                        case Constants.AddFigureOnce.figure_once:
                        case Constants.AddFigureOnce.figureonce:
                        case Constants.AddFigureOnce.fo:
                        case Constants.AddFigureOnce.o:
                            AddFigureOnce(parameters);
                            break;

                        case Constants.InitRotate.DefaultName:
                            InitRotate(parameters);
                            break;

                        case Constants.AddRotateAround.DefaultName:
                            AddRotateAround(parameters);
                            break;

                        case Constants.Rotate.DefaultName:
                            Rotate(parameters);
                            break;

                        case Constants.SetPoints.DefaultName:
                        case Constants.SetPoints.set_points:
                            SetPoints(parameters);
                            break;

                        case Constants.SetColor.DefaultName:
                        case Constants.SetColor.set_color:
                            SetColor(parameters);
                            break;

                        case Constants.AddFrame.DefaultName:
                        case Constants.AddFrame.add_frame:
                        case Constants.AddFrame.add:
                            AddFrame();
                            break;

                        case Constants.GoToFrame.DefaultName:
                        case Constants.GoToFrame.goto_frame:
                        case Constants.GoToFrame.GoTo:
                        case Constants.GoToFrame.@goto:
                            GoToFrame(parameters);
                            break;

                        case Constants.ChangeToNextFrame.DefaultName:
                        case Constants.ChangeToNextFrame.next_frame:
                        case Constants.ChangeToNextFrame.next:
                            ChangeToNextFrame(parameters);
                            break;

                        case Constants.ChangeToPreviousFrame.DefaultName:
                        case Constants.ChangeToPreviousFrame.previous_frame:
                        case Constants.ChangeToPreviousFrame.prev_frame:
                        case Constants.ChangeToPreviousFrame.prev:
                            ChangeToPreviousFrame();
                            break;

                        case Constants.ComputeXComputeY.DefaultName:
                        case Constants.ComputeXComputeY.cxcy:
                        case Constants.ComputeXComputeY.cpx_cpy:
                            ComputeXComputeY(parameters);
                            break;

                        case Constants.Cycle.DefaultName:
                            Cycle(parameters);
                            break;

                        case Constants.StopFileProcessing.DefaultName:
                        case Constants.StopFileProcessing.end:
                        case Constants.StopFileProcessing.x:
                        case Constants.StopFileProcessing.stop:
                        case Constants.StopFileProcessing.exit:
                        case Constants.StopFileProcessing.q:
                        case Constants.StopFileProcessing.quit:
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