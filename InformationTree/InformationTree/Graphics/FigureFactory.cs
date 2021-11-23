using System;

namespace InformationTree.Graphics
{
    public static class FigureFactory
    {
        public static BaseFigure GetFigure(string _line)
        {
            BaseFigure ret = null;

            var words = _line.Split(' ');

            if (words.Length < 3
            || !Enum.IsDefined(typeof(FigureType), words[0]))
                return ret;

            FigureType figureType = (FigureType)Enum.Parse(typeof(FigureType), words[0]);

            switch (figureType)
            {
                case FigureType.None:
                    break;

                case FigureType.Text:
                    switch (words.Length)
                    {
                        case 4:
                            // TextFigure Text X Y
                            ret = new TextFigure(words[1], int.Parse(words[2]), int.Parse(words[3]));
                            break;

                        case 7:
                            // TextFigure Text X Y R G B
                            ret = new TextFigure(words[1], int.Parse(words[2]),
                                int.Parse(words[3]), int.Parse(words[4]), int.Parse(words[5]), int.Parse(words[6]));
                            break;

                        case 9:
                            // TextFigure Text X Y Font Size R G B
                            ret = new TextFigure(
                                words[1], int.Parse(words[2]), int.Parse(words[3]),
                                words[4], int.Parse(words[5]), int.Parse(words[6]), int.Parse(words[7]), int.Parse(words[8]));
                            break;
                    }
                    break;

                case FigureType.Arc:
                    switch (words.Length)
                    {
                        case 3:
                            // ArcFigure X Y R ThetaFrom=0f ThetaTo=0.5f
                            ret = new ArcFigure(
                                double.Parse(words[1]),
                                double.Parse(words[2]),
                                double.Parse(words[3]),
                                0f,
                                0.5f);
                            break;

                        case 6:
                            // ArcFigure X Y R ThetaFrom ThetaTo
                            ret = new ArcFigure(
                                double.Parse(words[1]),
                                double.Parse(words[2]),
                                double.Parse(words[3]),
                                float.Parse(words[4]),
                                float.Parse(words[5]));
                            break;
                    }
                    break;

                case FigureType.Point:
                    switch (words.Length)
                    {
                        case 4:
                            // Point X Y Radius
                            ret = new Figure(1, double.Parse(words[1]), double.Parse(words[2]), double.Parse(words[3]));
                            break;

                        case 5:
                            // Point X Y Radius Rotation
                            ret = new Figure(1, double.Parse(words[1]), double.Parse(words[2]), double.Parse(words[3]), double.Parse(words[4]));
                            break;
                    }
                    break;

                case FigureType.Line:
                    switch (words.Length)
                    {
                        case 4:
                            // Line X Y Radius
                            ret = new Figure(2, double.Parse(words[1]), double.Parse(words[2]), double.Parse(words[3]));
                            break;

                        case 5:
                            // Line X Y Radius Rotation
                            ret = new Figure(2, double.Parse(words[1]), double.Parse(words[2]), double.Parse(words[3]), double.Parse(words[4]));
                            break;
                    }
                    break;
                    break;

                case FigureType.Circle:
                    switch (words.Length)
                    {
                        case 4:
                            // Circle X Y Radius
                            ret = new Figure(0, double.Parse(words[1]), double.Parse(words[2]), double.Parse(words[3]));
                            break;

                        case 5:
                            // Circle X Y Radius Rotation
                            ret = new Figure(0, double.Parse(words[1]), double.Parse(words[2]), double.Parse(words[3]), double.Parse(words[4]));
                            break;
                    }
                    break;

                case FigureType.Polygon:
                case FigureType.Figure:
                    switch (words.Length)
                    {
                        case 5:
                            // Figure Points X Y Radius
                            // Polygon Points X Y Radius
                            ret = new Figure(int.Parse(words[1]), double.Parse(words[2]), double.Parse(words[3]), double.Parse(words[4]));
                            break;

                        case 6:
                            // Figure Points X Y Radius Rotation
                            // Polygon Points X Y Radius Rotation
                            ret = new Figure(int.Parse(words[1]), double.Parse(words[2]), double.Parse(words[3]), double.Parse(words[4]), double.Parse(words[5]));
                            break;
                    }
                    break;
            }

            return ret;
        }
    }
}