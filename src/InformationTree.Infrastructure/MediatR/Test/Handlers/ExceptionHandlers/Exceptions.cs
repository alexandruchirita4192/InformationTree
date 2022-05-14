namespace InformationTree.Infrastructure.MediatR.Test.Handlers.ExceptionHandlers;

public class ConnectionException : Exception
{ }

public class ForbiddenException : ConnectionException
{ }

public class ResourceNotFoundException : ConnectionException
{ }

public class ServerException : Exception
{ }