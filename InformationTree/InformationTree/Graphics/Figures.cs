using System.Collections.Generic;
using System.Linq;
using D = System.Drawing;

namespace InformationTree.Graphics
{
    public class Figures // keep? or remove and add pointer to next figure @ Figure?
    {
        #region Properties

        public List<BaseFigure> FigureList { get; private set; }
        public D.Point CenterPoint { get; private set; }
        public D.Point RealCenterPoint { get; private set; }

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

        public void AddFigure(string s)
        {
            AddFigure(FigureFactory.GetFigure(s));
        }

        public void AddText(string s)
        {
            AddFigure(FigureFactory.GetFigure(s));
        }

        public void AddFigureOnce(BaseFigure figure)
        {
            if (FigureList != null && FigureList.FirstOrDefault(f => f.Points == figure.Points && f.X == figure.X && f.Y == figure.Y) == default(BaseFigure))
                FigureList.Add(figure);
        }

        public void AddFigureOnce(string s)
        {
            AddFigureOnce(FigureFactory.GetFigure(s));
        }

        // GoToNr(int nr) { iterates figures and gets figures with position "nr" }

        #region Individual methods

        public BaseFigure GetFigureAt(int _position)
        {
            if (FigureList != null && FigureList.Count > _position && _position >= 0)
                return FigureList.ElementAt(_position);
            return null;
        }

        public void InitRotate(int _position, double _rotation)
        {
            if (FigureList != null && FigureList.Count > _position && _position >= 0)
                FigureList.ElementAt(_position).InitRotate(_rotation);
        }

        public void InitRotate(string s)
        {
            var words = s.Split(' ');
            switch (words.Length)
            {
                case 2:
                    var _position = int.Parse(words[0]);
                    var _rotation = double.Parse(words[1]);
                    InitRotate(_position, _rotation);
                    break;
            }
        }

        public void Rotate(int _position)
        {
            if (FigureList != null && FigureList.Count > _position && _position >= 0)
                FigureList.ElementAt(_position).Rotate();
        }

        public void Rotate(string s)
        {
            Rotate(int.Parse(s));
        }

        public void RotateAround(int _position)
        {
            if (FigureList != null && FigureList.Count > _position && _position >= 0)
                FigureList.ElementAt(_position).RotateAround();
        }

        public void RotateAround(string s)
        {
            RotateAround(int.Parse(s));
        }

        public void AddRotateAround(int _position, double _r, double _addRotation)
        {
            if (FigureList != null && FigureList.Count > _position && _position >= 0)
                FigureList.ElementAt(_position).AddRotateAround(_r, _addRotation);
        }

        public void AddRotateAround(int _position, double _r, double _addRotation, double _angle)
        {
            if (FigureList != null && FigureList.Count > _position && _position >= 0)
                FigureList.ElementAt(_position).AddRotateAround(_r, _addRotation, _angle);
        }

        public void AddRotateAround(string s)
        {
            int _position;
            double _r, _addRotation;

            var words = s.Split(' ');
            switch (words.Length)
            {
                case 3:
                    _position = int.Parse(words[0]);
                    _r = double.Parse(words[1]);
                    _addRotation = double.Parse(words[2]);
                    AddRotateAround(_position, _r, _addRotation);
                    break;

                case 4:
                    _position = int.Parse(words[0]);
                    _r = double.Parse(words[1]);
                    _addRotation = double.Parse(words[2]);
                    var _angle = double.Parse(words[3]);
                    AddRotateAround(_position, _r, _addRotation, _angle);
                    break;
            }
        }

        public void ObsoleteAddRotateAround(string s)
        {
            var words = s.Split(' ');
            if (words.Length != 3)
                return;

            var _position = int.Parse(words[0]);
            var _addRotation = double.Parse(words[1]);
            var _r = double.Parse(words[2]);
            AddRotateAround(_position, _r, _addRotation);
        }

        public void ObsoleteAddRotateAround2(string s)
        {
            var words = s.Split(' ');
            if (words.Length != 4)
                return;

            var _position = int.Parse(words[0]);
            var _angle = double.Parse(words[1]);
            var _addRotation = double.Parse(words[2]);
            var _r = double.Parse(words[3]);
            AddRotateAround(_position, _r, _addRotation, _angle);
        }

        public void Move(int _position, double _x, double _y)
        {
            if (FigureList != null && FigureList.Count > _position && _position >= 0)
                FigureList.ElementAt(_position).Move(_x, _y);
        }

        public void Move(int _position, double _x, double _y, double _r)
        {
            if (FigureList != null && FigureList.Count > _position && _position >= 0)
            {
                var f = FigureList.ElementAt(_position);
                var ff = f as Figure;
                if (ff != null)
                    ff.Move(_x, _y, _r);
            }
        }

        public void Move(string s)
        {
            int _position;
            double _x, _y;
            var words = s.Split(' ');
            switch (words.Length)
            {
                case 3:
                    _position = int.Parse(words[0]);
                    _x = double.Parse(words[1]);
                    _y = double.Parse(words[2]);
                    Move(_position, _x, _y);
                    break;

                case 4:
                    _position = int.Parse(words[0]);
                    _x = double.Parse(words[1]);
                    _y = double.Parse(words[2]);
                    var _r = double.Parse(words[3]);
                    Move(_position, _x, _y, _r);
                    break;
            }        }

        public void SetPoints(int _position, int _points)
        {
            if (FigureList != null && FigureList.Count > _position && _position >= 0)
                FigureList.ElementAt(_position).Points = _points;
        }

        public void SetPoints(string s)
        {
            var words = s.Split(' ');
            switch (words.Length)
            {
                case 2:
                    var _position = int.Parse(words[0]);
                    var _points = int.Parse(words[1]);
                    SetPoints(_position, _points);
                    break;
            }
        }    
        public void SetColor(int _position, double _r, double _g, double _b)
        {
            if (FigureList != null && FigureList.Count > _position && _position >= 0)
                FigureList.ElementAt(_position).SetColor(_r, _g, _b);
        }

        public void SetColor(string s)
        {
            var words = s.Split(' ');
            switch (words.Length)
            {
                case 4:
                    var _position = int.Parse(words[0]);
                    var _r = double.Parse(words[1]);
                    var _g = double.Parse(words[2]);
                    var _b = double.Parse(words[3]);
                    SetColor(_position, _r, _g, _b);
                    break;
            }
        }

        public void Show(int _position, D.Graphics graphics)
        {
            if (FigureList != null && FigureList.Count > _position && _position >= 0)
                FigureList.ElementAt(_position).Show(graphics);
        }

        #endregion Individual methods

        #region Group methods

        public void InitRotateAll(double _rotation)
        {
            if (FigureList != null)
                FigureList.ForEach(f => f.InitRotate(_rotation));
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

        public void MoveAll(double _x, double _y)
        {
            if (FigureList != null)
                FigureList.ForEach(f => f.Move(_x, _y));
        }

        public void MoveAll(double _x, double _y, double _r)
        {
            if (FigureList != null)
                FigureList.ForEach(f =>
                {
                    // only Figure has a Radius (_r)
                    var ff = f as Figure;
                    if (ff != null)
                        ff.Move(_x, _y, _r);
                });
        }

        public void SetPointsAll(int _points)
        {
            FigureList.ForEach(f => f.Points = _points);
        }

        public void SetColorAll(double _r, double _g, double _b)
        {
            if (FigureList != null)
                FigureList.ForEach(f => f.SetColor(_r, _g, _b));
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

        public void TranslateCenterAll(D.Point oldCenter, D.Point newCenter)
        {
            if (oldCenter == null || newCenter == null)
                return;

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