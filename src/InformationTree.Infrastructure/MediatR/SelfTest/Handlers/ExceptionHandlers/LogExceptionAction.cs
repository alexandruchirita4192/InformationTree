using InformationTree.Infrastructure.MediatR.SelfTest.Requests;
using MediatR.Pipeline;

namespace InformationTree.Infrastructure.MediatR.SelfTest.Handlers.ExceptionHandlers;

public class LogExceptionAction : RequestExceptionAction<Ping>
{
    private readonly TextWriter _writer;

    public LogExceptionAction(TextWriter writer) => _writer = writer;

    protected override void Execute(Ping request, Exception exception)
    {
        _writer.WriteLineAsync($"--- Exception: '{exception.GetType().FullName}'");
    }
}
