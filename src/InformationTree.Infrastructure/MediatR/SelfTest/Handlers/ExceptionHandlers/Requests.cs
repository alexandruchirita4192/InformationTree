using InformationTree.Infrastructure.MediatR.SelfTest.Requests;

namespace InformationTree.Infrastructure.MediatR.SelfTest.Handlers.ExceptionHandlers;

public class PingResource : Ping
{ }

public class PingNewResource : Ping
{ }

public class PingResourceTimeout : PingResource
{ }

public class PingProtectedResource : PingResource
{ }