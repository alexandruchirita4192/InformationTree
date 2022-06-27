using System;

namespace InformationTree.Domain.Entities.Graphics
{
    public interface IGraphicsFile : IDisposable
    {
        void Clean();
        void ParseLines(string[] lines);
        void Show(System.Drawing.Graphics graphics);
        void GoToFrame(int position);
        void ChangeToPreviousFrame();
        void ChangeToNextFrame(bool addNextFrameIfNextFrameIsNull);
    }
}