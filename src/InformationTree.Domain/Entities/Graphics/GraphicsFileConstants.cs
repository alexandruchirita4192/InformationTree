namespace InformationTree.Domain.Entities.Graphics;

/// <summary>
/// Constants class with all commands (including some legacy commands)
/// </summary>
public static class GraphicsFileConstants
{
    public static class FrameRelativeToPoint
    {
        public const string DefaultName = $"{nameof(FrameRelativeToPoint)}";
        public const string FrameRelative = "FrameRelative";
        public const string FrameCenter = "FrameCenter";
    }

    public static class AllRelativeToPoint
    {
        public const string DefaultName = $"{nameof(AllRelativeToPoint)}";
        public const string RelativeToPoint = "RelativeToPoint";
        public const string Relative = "Relative";
        public const string Center = "Center";
        public const string AllRelative = "AllRelative";
        public const string AllCenter = "AllCenter";
    }

    public static class AddFigure
    {
        public const string DefaultName = $"{nameof(AddFigure)}";
        public const string f = "f";
        public const string figure = "figure";
    }

    public static class AddText
    {
        public const string DefaultName = $"{nameof(AddText)}";
        public const string t = "t";
        public const string txt = "txt";
        public const string text = "text";
    }

    public static class AddFigureOnce
    {
        public const string DefaultName = $"{nameof(AddFigureOnce)}";
        public const string o = "o";
        public const string fo = "fo";
        public const string figureonce = "figureonce";
        public const string figure_once = "figure_once";
    }

    public static class InitRotate
    {
        public const string DefaultName = $"{nameof(InitRotate)}";
    }

    public static class AddRotateAround
    {
        public const string DefaultName = $"{nameof(AddRotateAround)}";
    }

    public static class Rotate
    {
        public const string DefaultName = $"{nameof(Rotate)}";
    }

    public static class SetPoints
    {
        public const string DefaultName = $"{nameof(SetPoints)}";
        public const string set_points = "set_points";
    }

    public static class SetColor
    {
        public const string DefaultName = $"{nameof(SetColor)}";
        public const string set_color = "set_color";
    }

    public static class AddFrame
    {
        public const string DefaultName = $"{nameof(AddFrame)}";
        public const string add_frame = "add_frame";
        public const string add = "add";
    }

    public static class GoToFrame
    {
        public const string DefaultName = $"{nameof(GoToFrame)}";
        public const string goto_frame = "goto_frame";
        public const string GoTo = "GoTo";
        public const string @goto = "goto";
    }

    public static class ChangeToNextFrame
    {
        public const string DefaultName = $"{nameof(ChangeToNextFrame)}";
        public const string next_frame = "next_frame";
        public const string next = "next";
    }

    public static class ChangeToPreviousFrame
    {
        public const string DefaultName = $"{nameof(ChangeToPreviousFrame)}";
        public const string previous_frame = "previous_frame";
        public const string prev_frame = "prev_frame";
        public const string prev = "prev";
    }

    public static class GenerateFigureLines
    {
        public const string DefaultName = $"{nameof(GenerateFigureLines)}";
        public const string ComputeXComputeY = "ComputeXComputeY";
        public const string cxcy = "cxcy";
        public const string cpx_cpy = "cpx_cpy";
    }

    public static class Cycle
    {
        public const string DefaultName = $"{nameof(Cycle)}";
    }

    public static class StopFileProcessing
    {
        public const string DefaultName = $"{nameof(StopFileProcessing)}";
        public const string end = "end";
        public const string x = "x";
        public const string stop = "stop";
        public const string exit = "exit";
        public const string q = "q";
        public const string quit = "quit";
    }
}
