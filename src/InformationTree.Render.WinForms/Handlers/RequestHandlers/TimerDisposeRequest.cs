using System.ComponentModel;
using System.Timers;
using InformationTree.Domain.Requests.Base;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers;

public class TimerDisposeRequest : BaseRequest
{
    public Component Timer { get; set; }

    public ElapsedEventHandler ElapsedEventHandler { get; set; }
}