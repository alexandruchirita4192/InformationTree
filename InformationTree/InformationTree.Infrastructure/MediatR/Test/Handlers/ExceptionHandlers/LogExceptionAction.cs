using InformationTree.Infrastructure.MediatR.Test.Requests;
using MediatR.Pipeline;

namespace InformationTree.Infrastructure.MediatR.Test.Handlers.ExceptionHandlers;

public class LogExceptionAction : RequestExceptionAction<Ping>
{
    private readonly TextWriter _writer;

    public LogExceptionAction(TextWriter writer) => _writer = writer;

    protected override void Execute(Ping request, Exception exception)
    {
        _writer.WriteLineAsync($"--- Exception: '{exception.GetType().FullName}'");
    }
}
