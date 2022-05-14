using InformationTree.Infrastructure.MediatR.Test.Requests;

namespace InformationTree.Infrastructure.MediatR.Test.Handlers.ExceptionHandlers;

public class PingResource : Ping
{ }

public class PingNewResource : Ping
{ }

public class PingResourceTimeout : PingResource
{ }

public class PingProtectedResource : PingResource
{ }