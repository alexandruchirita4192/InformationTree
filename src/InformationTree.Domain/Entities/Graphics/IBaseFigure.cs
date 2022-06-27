namespace InformationTree.Domain.Entities.Graphics
{
    public interface IBaseFigure
    {
        int Points { get; set; }
        double X { get; }
        double Y { get; }

        void InitRotate(double rotation);
        void Rotate();
        void RotateAround();
        void AddRotateAround(double radius, double addRotation);
        void AddRotateAround(double radius, double addRotation, double angle);
        void Move(double x, double y);
        void SetColor(double red, double green, double blue);
        void Show(System.Drawing.Graphics graphics);
    }
}