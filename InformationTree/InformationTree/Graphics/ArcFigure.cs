using D=System.Drawing;

namespace InformationTree.Graphics
{
    public class ArcFigure : Figure
    {
        #region Properties

        public override FigureType FigureType
        {
            get
            {
                return FigureType.Arc;
            }
        }

        public float ThetaFrom { get; private set; }
        public float ThetaTo { get; private set; }

        #endregion Properties

        #region Constructors

        public ArcFigure()
        {
            ThetaFrom = 0.0f;
            ThetaTo = 0.0f;
        }

        public ArcFigure(double x, double y, double r, float thetaFrom, float thetaTo)
        {
            X = x;
            Y = y;
            Radius = r;
            ThetaFrom = thetaFrom;
            ThetaTo = thetaTo;
        }

        #endregion Constructors

        #region Methods

        public void ChangeTheta(float thetaFrom, float thetaTo)
        {
            ThetaFrom = thetaFrom;
            ThetaTo = thetaTo;
        }

        public override void Show(System.Drawing.Graphics graphics)
        {
            var color = D.Color.FromArgb((int)(Red * 255.0), (int)(Green * 255.0), (int)(Blue * 255.0));
            var pen = new D.Pen(color);
            graphics.DrawArc(pen, new D.Rectangle(new D.Point((int)X, (int)Y), new D.Size((int)Radius, (int)Radius)), ThetaFrom, ThetaTo);
        }

        public override string GetDebugText()
        {
            return string.Format("ThetaFrom={0}; ThetaTo={1};", ThetaFrom, ThetaTo);
        }

        #endregion Methods
    }
}