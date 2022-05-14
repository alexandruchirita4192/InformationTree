using System;

namespace InformationTree.Domain.Entities.Graphics
{
    public interface IGraphicsFile : IDisposable
    {
        void Clean();
        void ParseLines(string[] lines);
        void Show(System.Drawing.Graphics graphics);
        void GoToFrame(string frame);
        void ChangeToPreviousFrame();
        void ChangeToNextFrame(string v);
    }
}