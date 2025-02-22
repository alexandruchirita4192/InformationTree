using System;

namespace InformationTree.Domain.Entities.Graphics
{
    public interface ICanvasForm : IDisposable
    {
        IGraphicsFile GraphicsFile { get; }
        System.Timers.Timer RunTimer { get; }
        bool IsDisposed { get; }
        void Show();
        void Close();
    }
}