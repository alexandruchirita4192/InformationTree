using InformationTree.Domain;
using InformationTree.Extra.Graphics.Domain;

namespace InformationTree.Extra.Graphics.Services
{
    public static class FigureFactory
    {
        public static BaseFigure? GetFigure(string _line)
        {
            BaseFigure? ret = null;

            var words = _line.Split(Constants.Parsing.SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length < 3
            || !Enum.IsDefined(typeof(FigureType), words[0]))
                return ret;

            var figureType = (FigureType)Enum.Parse(typeof(FigureType), words[0]);

            // Common variables declared beforehand even if not all of them are used by all switch statements
            // (cannot declare local variables with the same name even if it's in another switch statement)
            double x, y, radius, rotation;
            int red, green, blue, points;

            switch (figureType)
            {
                case FigureType.None:
                    break;

                case FigureType.Text:
                    switch (words.Length)
                    {
                        case 4:
                            // TextFigure Text X Y
                            if (!double.TryParse(words[2], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");

                            ret = new TextFigure(words[1], x, y);
                            break;

                        case 7:
                            // TextFigure Text X Y R G B
                            if (!double.TryParse(words[2], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");
                            if (!int.TryParse(words[4], out red))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[4]}' as {typeof(int).Name}.");
                            if (!int.TryParse(words[5], out green))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[5]}' as {typeof(int).Name}.");
                            if (!int.TryParse(words[6], out blue))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[6]}' as {typeof(int).Name}.");

                            ret = new TextFigure(words[1], x, y, red, green, blue);
                            break;

                        case 9:
                            // TextFigure Text X Y Font Size R G B
                            if (!double.TryParse(words[2], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");
                            if (!int.TryParse(words[5], out var size))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[5]}' as {typeof(int).Name}.");
                            if (!int.TryParse(words[6], out red))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[6]}' as {typeof(int).Name}.");
                            if (!int.TryParse(words[7], out green))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[7]}' as {typeof(int).Name}.");
                            if (!int.TryParse(words[8], out blue))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[8]}' as {typeof(int).Name}.");

                            ret = new TextFigure(words[1], x, y, words[4], size, red, green, blue);
                            break;
                    }
                    break;

                case FigureType.Arc:
                    switch (words.Length)
                    {
                        case 3:
                            // ArcFigure X Y R ThetaFrom=0f ThetaTo=0.5f
                            if (!double.TryParse(words[1], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[1]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[2], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out radius))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");

                            ret = new ArcFigure(x, y, radius, 0f, 0.5f);
                            break;

                        case 6:
                            // ArcFigure X Y R ThetaFrom ThetaTo
                            if (!double.TryParse(words[1], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[1]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[2], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out radius))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");
                            if (!float.TryParse(words[4], out var thetaFrom))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[4]}' as {typeof(float).Name}.");
                            if (!float.TryParse(words[5], out var thetaTo))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[5]}' as {typeof(float).Name}.");

                            ret = new ArcFigure(x, y, radius, thetaFrom, thetaTo);
                            break;
                    }
                    break;

                case FigureType.Point:
                    switch (words.Length)
                    {
                        case 4:
                            // Point X Y Radius
                            if (!double.TryParse(words[1], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[1]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[2], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out radius))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");
                            
                            ret = new Figure(1, x, y, radius);
                            break;

                        case 5:
                            // Point X Y Radius Rotation
                            if (!double.TryParse(words[1], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[1]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[2], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out radius))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[4], out rotation))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[4]}' as {typeof(double).Name}.");

                            ret = new Figure(1, x, y, radius, rotation);
                            break;
                    }
                    break;

                case FigureType.Line:
                    switch (words.Length)
                    {
                        case 4:
                            // Line X Y Radius
                            if (!double.TryParse(words[1], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[1]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[2], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out radius))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");
                            
                            ret = new Figure(2, x, y, radius);
                            break;

                        case 5:
                            // Line X Y Radius Rotation
                            if (!double.TryParse(words[1], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[1]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[2], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out radius))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[4], out rotation))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[4]}' as {typeof(double).Name}.");

                            ret = new Figure(2, x, y, radius, rotation);
                            break;
                    }
                    break;

                case FigureType.Circle:
                    switch (words.Length)
                    {
                        case 4:
                            // Circle X Y Radius
                            if (!double.TryParse(words[1], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[1]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[2], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out radius))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");

                            ret = new Figure(0, x, y, radius);
                            break;

                        case 5:
                            // Circle X Y Radius Rotation
                            if (!double.TryParse(words[1], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[1]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[2], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out radius))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[4], out rotation))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[4]}' as {typeof(double).Name}.");

                            ret = new Figure(0, x, y, radius, rotation);
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
                            if (!int.TryParse(words[1], out points))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[1]}' as {typeof(int).Name}.");
                            if (!double.TryParse(words[2], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[4], out radius))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[4]}' as {typeof(double).Name}.");

                            ret = new Figure(points, x, y, radius);
                            break;

                        case 6:
                            // Figure Points X Y Radius Rotation
                            // Polygon Points X Y Radius Rotation
                            if (!int.TryParse(words[1], out points))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[1]}' as {typeof(int).Name}.");
                            if (!double.TryParse(words[2], out x))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[2]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[3], out y))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[3]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[4], out radius))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[4]}' as {typeof(double).Name}.");
                            if (!double.TryParse(words[5], out rotation))
                                throw new Exception($"{nameof(FigureFactory)}.{nameof(GetFigure)} parsing issue for figure type {figureType} for '{words[5]}' as {typeof(double).Name}.");

                            ret = new Figure(points, x, y, radius, rotation);
                            break;
                    }
                    break;
            }

            return ret;
        }
    }
}