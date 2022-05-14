using System;
using System.Timers;

namespace InformationTree.Domain.Entities.Graphics
{
    public interface ICanvasForm : IDisposable
    {
        IGraphicsFile GraphicsFile { get; }
        Timer RunTimer { get; }
        bool IsDisposed { get; }
        void Show();
        void Close();
    }
}