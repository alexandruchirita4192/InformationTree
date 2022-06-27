using System.Collections.Generic;
using System.Drawing;

namespace InformationTree.Domain.Entities.Graphics
{
    public interface IFigures
    {
        public List<IBaseFigure> FigureList { get; }
        Point CenterPoint { get; }
        Point RealCenterPoint { get; }

        void AddFigure(IBaseFigure figure);
        void AddText(string parameters);
        void AddFigureOnce(IBaseFigure? baseFigure);
        void InitRotate(string parameters);
        void AddRotateAround(string parameters);
        void Rotate(string parameters);
        void SetPoints(string parameters);
        void SetColor(string parameters);
        void ShowAll(System.Drawing.Graphics graphics);
        void TranslateCenterAll(Point oldCenter, Point newCenter);
    }
}