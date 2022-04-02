using D = System.Drawing;

namespace InformationTree.Extra.Graphics.Extensions
{
    public static class GraphicsExtensions
    {
        public static void DrawCircle(this D.Graphics g, D.Pen pen, float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius, radius + radius, radius + radius);
        }

        public static void FillCircle(this D.Graphics g, D.Brush brush, float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius, radius + radius, radius + radius);
        }
    }
}