using System.Text;
using InformationTree.Extra.Graphics.Computation;
using InformationTree.Graphics;
using D = System.Drawing;
using InformationTree.Domain.Services;

namespace InformationTree.Extra.Graphics.Services.FileParsing
{
    [Obsolete("Break into many classes later")] // TODO: file parsing in one file, figures drawing in another
    public class GraphicsFile : IDisposable
    {
        private IGraphicsProvider _graphicsProvider;

        #region Properties

        public Frame Frame { get; private set; }
        public System.Threading.Timer RunTimer { get; set; }

        #endregion Properties

        #region Constructor

        public GraphicsFile(IGraphicsProvider graphicsProvider)
        {
            _graphicsProvider = graphicsProvider;

            Frame = new Frame();
            var interval = 1000 / GraphicsComputation.FramesPerSecond;
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
                        case "FrameRelativeToPoint":
                        case "FrameRelative":
                        case "FrameCenter":
                            FrameRelativeToPoint(parameters);
                            break;

                        case "RelativeToPoint":
                        case "Relative":
                        case "Center":
                        case "AllRelativeToPoint":
                        case "AllRelative":
                        case "AllCenter":
                            AllRelativeToPoint(parameters);
                            break;

                        case "f":
                        case "figure":
                        case "AddFigure":
                            AddFigure(parameters);
                            break;

                        case "text":
                        case "txt":
                        case "t":
                        case "AddText":
                            AddText(parameters);
                            break;

                        case "o":
                        case "figure_once":
                        case "AddFigureOnce":
                            AddFigureOnce(parameters);
                            break;

                        case "InitRotate":
                            InitRotate(parameters);
                            break;

                        case "AddRotateAround":
                            AddRotateAround(parameters);
                            break;

                        case "Rotate":
                            Rotate(parameters);
                            break;

                        case "add_rotate_around":
                            ObsoleteAddRotateAround(parameters);
                            break;

                        case "add_rotate_around2":
                            ObsoleteAddRotateAround2(parameters);
                            break;

                        case "set_points":
                        case "SetPoints":
                            SetPoints(parameters);
                            break;

                        case "set_color":
                        case "SetColor":
                            SetColor(parameters);
                            break;

                        case "add":
                        case "add_frame":
                        case "AddFrame":
                            AddFrame();
                            break;

                        case "goto_frame":
                        case "GoToFrame":
                        case "GoTo":
                        case "goto":
                            GoToFrame(parameters);
                            break;

                        case "next_frame":
                        case "next":
                        case "ChangeToNextFrame":
                            ChangeToNextFrame(parameters);
                            break;

                        case "previous_frame":
                        case "prev_frame":
                        case "prev":
                        case "ChangeToPreviousFrame":
                            ChangeToPreviousFrame();
                            break;

                        case "cxcy":
                        case "cpx_cpy":
                        case "ComputeXComputeY":
                            ComputeXComputeY(parameters);
                            break;

                        case "Cycle":
                            Cycle(parameters);
                            break;

                        case "end":
                        case "x":
                        case "stop":
                        case "exit":
                        case "q":
                        case "quit":
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
                D.Point oldPoint = new D.Point(), newPoint = new D.Point();

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
            int _iterations, _computeType;

            switch (words.Length)
            {
                case 2:
                case 3:
                    _radius = double.Parse(words[0]);
                    _iterations = int.Parse(words[1]);
                    _computeType = words.Length == 3 ? int.Parse(words[2]) : 0;
                    ParseLines(_graphicsProvider.ComputeXComputeY(_radius, _iterations, _computeType).Distinct().ToArray());
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
                    _computeType = words.Length == 8 ? int.Parse(words[7]) : 0;
                    ParseLines(_graphicsProvider.ComputeXComputeY(_points, _x, _y, _radius, _theta, _number, _iterations, _computeType).Distinct().ToArray());
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

        public void ObsoleteAddRotateAround(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.ObsoleteAddRotateAround(s);
        }

        public void ObsoleteAddRotateAround2(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            if (Frame == null)
                return;
            var figures = Frame.GetActiveFrameOrThis().Figures;
            if (figures == null)
                return;
            figures.ObsoleteAddRotateAround2(s);
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