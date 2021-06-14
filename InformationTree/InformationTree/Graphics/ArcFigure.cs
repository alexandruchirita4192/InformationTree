using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationTree.Graphics
{
    public class ArcFigure : BaseFigure
    {

        public override FigureType FigureType
        {
            get
            {
                return FigureType.Arc;
            }
        }

        public double? ThetaFrom { get; private set; }
        public double? ThetaTo { get; private set; }

        public ArcFigure()
        {
            ThetaFrom = 0.0;
            ThetaTo = 0.0;
        }

        public void ChangeTheta(double thetaFrom, double thetaTo)
        {
            ThetaFrom = thetaFrom;
            ThetaTo = thetaTo;
        }

        public override void Show(System.Drawing.Graphics graphics)
        {

        }

        public override string GetDebugText()
        {
            return string.Format("ThetaFrom={0}; ThetaTo={1};", ThetaFrom, ThetaTo);
        }
    }
}
