using InformationTree.Domain.Extensions;
using InformationTree.Extra.Graphics.Services;
using D = System.Drawing;

namespace InformationTree.Extra.Graphics.Domain
{
    public class Figures // keep? or remove and add pointer to next figure @ Figure?
    {
        #region Properties

        public List<BaseFigure> FigureList { get; private set; }
        public Point CenterPoint { get; private set; }
        public Point RealCenterPoint { get; private set; }

        #endregion Properties

        #region Constructor

        public Figures()
        {
            FigureList = new List<BaseFigure>();
        }

        public Figures(BaseFigure figure) : this()
        {
            AddFigure(figure);
        }

        #endregion Constructor

        #region Methods

        public void AddFigure(BaseFigure figure)
        {
            if (FigureList != null)
                FigureList.Add(figure);
        }

        public void AddFigure(string figureLine)
        {
            if (figureLine.IsEmpty())
                return;
            var figure = FigureFactory.GetFigure(figureLine);
            if (figure == null)
                return;
            AddFigure(figure);
        }

        public void AddText(string textLine)
        {
            AddFigure(textLine);
        }

        public void AddFigureOnce(BaseFigure figure)
        {
            if (FigureList != null && FigureList.FirstOrDefault(f => f.Points == figure.Points && f.X == figure.X && f.Y == figure.Y) == default(BaseFigure))
                FigureList.Add(figure);
        }

        public void AddFigureOnce(string figureLine)
        {
            if (figureLine.IsEmpty())
                return;
            var figure = FigureFactory.GetFigure(figureLine);
            if (figure == null)
                return;
            AddFigureOnce(figure);
        }

        // GoToNr(int nr) { iterates figures and gets figures with position "nr" }

        #region Individual methods

        public BaseFigure? GetFigureAt(int position)
        {
            return FigureList != null && FigureList.Count > position && position >= 0 ? FigureList.ElementAt(position) : null;
        }

        public void InitRotate(int position, double rotation)
        {
            if (FigureList != null && FigureList.Count > position && position >= 0)
                FigureList.ElementAt(position).InitRotate(rotation);
        }

        public void InitRotate(string parameters)
        {
            if (parameters.IsEmpty())
                return;
            
            var words = parameters.Split(' ');
            switch (words.Length)
            {
                case 2:
                    if (int.TryParse(words[0], out var position)
                    && double.TryParse(words[1], out var rotation))
                        InitRotate(position, rotation);
                    break;
            }
        }

        public void Rotate(int position)
        {
            if (FigureList != null && FigureList.Count > position && position >= 0)
                FigureList.ElementAt(position).Rotate();
        }

        public void Rotate(string positionStr)
        {
            Rotate(int.Parse(positionStr));
        }

        public void RotateAround(int position)
        {
            if (FigureList != null && FigureList.Count > position && position >= 0)
                FigureList.ElementAt(position).RotateAround();
        }

        public void RotateAround(string s)
        {
            if (int.TryParse(s, out int result))
                RotateAround(result);
        }

        public void AddRotateAround(int position, double r, double addRotation)
        {
            if (FigureList != null && FigureList.Count > position && position >= 0)
                FigureList.ElementAt(position).AddRotateAround(r, addRotation);
        }

        public void AddRotateAround(int position, double r, double addRotation, double angle)
        {
            if (FigureList != null && FigureList.Count > position && position >= 0)
                FigureList.ElementAt(position).AddRotateAround(r, addRotation, angle);
        }

        public void AddRotateAround(string parameters)
        {
            if (parameters.IsEmpty())
                return;

            int position;
            double r, addRotation;

            var words = parameters.Split(' ');
            switch (words.Length)
            {
                case 3:
                    if (int.TryParse(words[0], out position)
                    && double.TryParse(words[1], out r)
                    && double.TryParse(words[2], out addRotation))
                        AddRotateAround(position, r, addRotation);
                    break;

                case 4:
                    if (int.TryParse(words[0], out position)
                    && double.TryParse(words[1], out r)
                    && double.TryParse(words[2], out addRotation)
                    && double.TryParse(words[3], out var angle))
                        AddRotateAround(position, r, addRotation, angle);
                    break;
            }
        }

        public void Move(int position, double x, double y)
        {
            if (FigureList != null && FigureList.Count > position && position >= 0)
                FigureList.ElementAt(position).Move(x, y);
        }

        public void Move(int position, double x, double y, double r)
        {
            if (FigureList != null && FigureList.Count > position && position >= 0)
            {
                var f = FigureList.ElementAt(position);
                if (f is Figure ff)
                    ff.Move(x, y, r);
            }
        }

        public void Move(string s)
        {
            int position;
            double x, y;
            var words = s.Split(' ');
            switch (words.Length)
            {
                case 3:
                    if (int.TryParse(words[0], out position)
                    && double.TryParse(words[1], out x)
                    && double.TryParse(words[2], out y))
                        Move(position, x, y);
                    break;

                case 4:
                    if (int.TryParse(words[0], out position)
                    && double.TryParse(words[1], out x)
                    && double.TryParse(words[2], out y)
                    && double.TryParse(words[3], out var r))
                        Move(position, x, y, r);
                    break;
            }
        }

        public void SetPoints(int position, int points)
        {
            if (FigureList != null && FigureList.Count > position && position >= 0)
                FigureList.ElementAt(position).Points = points;
        }

        public void SetPoints(string parameters)
        {
            var words = parameters.Split(' ');
            switch (words.Length)
            {
                case 2:
                    var position = int.Parse(words[0]);
                    var points = int.Parse(words[1]);
                    SetPoints(position, points);
                    break;
            }
        }

        public void SetColor(int position, double r, double g, double b)
        {
            if (FigureList != null && FigureList.Count > position && position >= 0)
                FigureList.ElementAt(position).SetColor(r, g, b);
        }

        public void SetColor(string parameters)
        {
            var words = parameters.Split(' ');
            switch (words.Length)
            {
                case 4:
                    if (int.TryParse(words[0], out var position)
                    && double.TryParse(words[1], out var r)
                    && double.TryParse(words[2], out var g)
                    && double.TryParse(words[3], out var b))
                        SetColor(position, r, g, b);
                    break;
            }
        }

        public void Show(int position, D.Graphics graphics)
        {
            if (FigureList != null && FigureList.Count > position && position >= 0)
                FigureList.ElementAt(position).Show(graphics);
        }

        #endregion Individual methods

        #region Group methods

        public void InitRotateAll(double rotation)
        {
            if (FigureList != null)
                FigureList.ForEach(f => f.InitRotate(rotation));
        }

        public void RotateAll()
        {
            if (FigureList != null)
                FigureList.ForEach(f => f.Rotate());
        }

        public void RotateAroundAll()
        {
            if (FigureList != null)
                FigureList.ForEach(f => f.RotateAround());
        }

        public void MoveAll(double x, double y)
        {
            if (FigureList != null)
                FigureList.ForEach(f => f.Move(x, y));
        }

        public void MoveAll(double x, double y, double r)
        {
            if (FigureList != null)
                FigureList.ForEach(f =>
                {
                    // only Figure has a Radius (r)
                    if (f is Figure ff)
                        ff.Move(x, y, r);
                });
        }

        public void SetPointsAll(int points)
        {
            FigureList.ForEach(f => f.Points = points);
        }

        public void SetColorAll(double r, double g, double b)
        {
            if (FigureList != null)
                FigureList.ForEach(f => f.SetColor(r, g, b));
        }

        public void ShowAll(D.Graphics graphics)
        {
            if (FigureList != null)
                FigureList.ForEach(f =>
                {
                    f.Show(graphics);
                    f.RotateAround();
                    f.Rotate();
                });
        }

        public void TranslateCenterAll(Point oldCenter, Point newCenter)
        {
            CenterPoint = oldCenter;
            RealCenterPoint = newCenter;

            if (FigureList != null)
                FigureList.ForEach(f =>
                {
                    f.Move(
                        f.X + RealCenterPoint.X - CenterPoint.X,
                        f.Y + RealCenterPoint.Y - CenterPoint.Y
                    );
                });
        }

        #endregion Group methods

        #endregion Methods
    }
}