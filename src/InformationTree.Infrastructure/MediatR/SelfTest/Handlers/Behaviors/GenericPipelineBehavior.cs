﻿using MediatR;

namespace InformationTree.Infrastructure.MediatR.SelfTest.Handlers.Behaviors;

public class GenericPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
{
    private readonly TextWriter _writer;

    public GenericPipelineBehavior(TextWriter writer)
    {
        _writer = writer;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        await _writer.WriteLineAsync("-- Handling Request");
        var response = await next();
        await _writer.WriteLineAsync("-- Finished Request");
        return response;
    }
}
