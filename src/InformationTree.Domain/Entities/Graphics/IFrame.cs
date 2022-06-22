using System.Drawing;

namespace InformationTree.Domain.Entities.Graphics
{
    public interface IFrame
    {
        IFigures Figures { get; }

        void TranslateCenter(Point oldCenter, Point newCenter);

        void TranslateCenterAllFrames(Point oldCenter, Point newCenter);

        IFrame GetActiveFrameOrThis();

        void NewFrame();

        void GoToFrame(int position);

        bool ChangeToNextFrame(bool addNextFrameIfNextFrameIsNull);

        void ChangeToPreviousFrame();
    }
}