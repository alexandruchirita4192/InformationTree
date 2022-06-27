using System.Diagnostics;
using System.Reflection;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using InformationTree.Infrastructure.MediatR.SelfTest.Handlers.Behaviors;
using InformationTree.Infrastructure.MediatR.SelfTest.Handlers.EventHandlers;
using InformationTree.Infrastructure.MediatR.SelfTest.Handlers.PostProcessors;
using InformationTree.Infrastructure.MediatR.SelfTest.Handlers.PreProcessors;
using InformationTree.Infrastructure.MediatR.SelfTest.Requests;
using MediatR;
using MediatR.Pipeline;

namespace InformationTree.Infrastructure.MediatR;

public static class WindsorContainerExtension
{
    public static IMediator BuildMediatorForSelfTest(this IWindsorContainer container, StringWriter writer)
    {

        // *** The default lifestyle for Windsor is Singleton
        // *** If you are using ASP.net, it's better to register your services with 'Per Web Request LifeStyle'.

        var fromAssemblyContainingPing = Classes.FromAssemblyContaining<Ping>();
        container.RegisterHandlersProcessorsAndActionsFor(fromAssemblyContainingPing);
        
        container.Register(Component.For<TextWriter>().Instance(writer));

        //Pipeline
        container.Register(Component.For(typeof(IStreamPipelineBehavior<,>)).ImplementedBy(typeof(GenericStreamPipelineBehavior<,>)));
        container.Register(Component.For(typeof(IPipelineBehavior<,>)).ImplementedBy(typeof(GenericPipelineBehavior<,>)).NamedAutomatically("Pipeline"));
        container.Register(Component.For(typeof(IRequestPreProcessor<>)).ImplementedBy(typeof(GenericRequestPreProcessor<>)).NamedAutomatically("PreProcessor"));
        container.Register(Component.For(typeof(IRequestPostProcessor<,>)).ImplementedBy(typeof(GenericRequestPostProcessor<,>)).NamedAutomatically("PostProcessor"));
        container.Register(Component.For(typeof(IRequestPostProcessor<,>), typeof(ConstrainedRequestPostProcessor<,>)).NamedAutomatically("ConstrainedRequestPostProcessor"));
        container.Register(Component.For(typeof(INotificationHandler<>), typeof(ConstrainedPingedHandler<>)).NamedAutomatically("ConstrainedPingedHandler"));

        var mediator = container.Resolve<IMediator>();

        return mediator;
    }

    public static void RegisterMediatorForUsageFrom(this IWindsorContainer container, Assembly assembly)
    {
        container.AddKernelSubResolverAndHandlersFilter();
        
        container.RegisterHandlersProcessorsAndActionsFor(Classes.FromAssembly(assembly));
        container.RegisterMediatorAndServiceFactory();
        
        // Pipeline
        container.RegisterGenericMediatorPipeline();
    }

    private static void AddKernelSubResolverAndHandlersFilter(this IWindsorContainer container)
    {
        container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
        container.Kernel.AddHandlersFilter(new ContravariantFilter());
    }

    private static void RegisterMediatorAndServiceFactory(this IWindsorContainer container)
    {
        container.Register(Component.For<IMediator>().ImplementedBy<Mediator>());
        container.Register(Component.For<ServiceFactory>().UsingFactoryMethod<ServiceFactory>(k => (type =>
        {
            var enumerableType = type
                .GetInterfaces()
                .Concat(new[] { type })
                .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            var service = enumerableType?.GetGenericArguments()?[0];
            var resolvedType = enumerableType != null ? k.ResolveAll(service) : k.Resolve(type);
            var genericArguments = service?.GetGenericArguments();

            // Handle exceptions even using the base request types for IRequestExceptionHandler<,,>
            var isRequestExceptionHandler = service?.GetGenericTypeDefinition()
                ?.IsAssignableTo(typeof(IRequestExceptionHandler<,,>)) ?? false;
            if (isRequestExceptionHandler)
                return ResolveRequestExceptionHandler(k, type, service, resolvedType, genericArguments);

            // Handle exceptions even using the base request types for IRequestExceptionAction<,>
            var isRequestExceptionAction = service?.GetGenericTypeDefinition()
                ?.IsAssignableTo(typeof(IRequestExceptionAction<,>)) ?? false;
            if (isRequestExceptionAction)
                return ResolveRequestExceptionAction(k, type, service, resolvedType, genericArguments);

            return resolvedType;
        })));
    }

    private static void RegisterHandlersProcessorsAndActionsFor(this IWindsorContainer container, FromAssemblyDescriptor fromAssemblyContainingPing)
    {
        container.Register(fromAssemblyContainingPing.BasedOn(typeof(IRequestHandler<,>)).WithServiceAllInterfaces().AllowMultipleMatches());
        container.Register(fromAssemblyContainingPing.BasedOn(typeof(INotificationHandler<>)).WithServiceAllInterfaces().AllowMultipleMatches());
        container.Register(fromAssemblyContainingPing.BasedOn(typeof(IRequestExceptionAction<,>)).WithServiceAllInterfaces().AllowMultipleMatches());
        container.Register(fromAssemblyContainingPing.BasedOn(typeof(IRequestExceptionHandler<,,>)).WithServiceAllInterfaces().AllowMultipleMatches());
        container.Register(fromAssemblyContainingPing.BasedOn(typeof(IStreamRequestHandler<,>)).WithServiceAllInterfaces().AllowMultipleMatches());
        container.Register(fromAssemblyContainingPing.BasedOn(typeof(IRequestPreProcessor<>)).WithServiceAllInterfaces().AllowMultipleMatches());
        container.Register(fromAssemblyContainingPing.BasedOn(typeof(IRequestPostProcessor<,>)).WithServiceAllInterfaces().AllowMultipleMatches());
    }

    private static void RegisterGenericMediatorPipeline(this IWindsorContainer container)
    {
        container.Register(Component.For(typeof(IPipelineBehavior<,>)).ImplementedBy(typeof(RequestExceptionProcessorBehavior<,>)));
        container.Register(Component.For(typeof(IPipelineBehavior<,>)).ImplementedBy(typeof(RequestExceptionActionProcessorBehavior<,>)));
        container.Register(Component.For(typeof(IPipelineBehavior<,>)).ImplementedBy(typeof(RequestPreProcessorBehavior<,>)).NamedAutomatically("PreProcessorBehavior"));
        container.Register(Component.For(typeof(IPipelineBehavior<,>)).ImplementedBy(typeof(RequestPostProcessorBehavior<,>)).NamedAutomatically("PostProcessorBehavior"));
    }

    private static object ResolveRequestExceptionHandler(IKernel k, Type type, Type service, object resolvedType, Type[] genericArguments)
    {
        if (service == null
        || genericArguments == null
        || !service.IsInterface
        || !service.IsGenericType
        || !service.IsConstructedGenericType
        || !(service.GetGenericTypeDefinition()
        ?.IsAssignableTo(typeof(IRequestExceptionHandler<,,>)) ?? false)
        || genericArguments.Length != 3)
        {
            return resolvedType;
        }

        var serviceFactory = k.Resolve<ServiceFactory>();
        var baseRequestType = genericArguments[0].BaseType;
        var response = genericArguments[1];
        var exceptionType = genericArguments[2];

        // Check if the base request type is valid
        if (baseRequestType == null
        || !baseRequestType.IsClass
        || baseRequestType == typeof(object)
        || ((!baseRequestType.GetInterfaces()
            ?.Any(i => i.IsAssignableFrom(typeof(IRequest<>)))) ?? true))
        {
            return resolvedType;
        }

        var exceptionHandlerInterfaceType = typeof(IRequestExceptionHandler<,,>).MakeGenericType(baseRequestType, response, exceptionType);
        var enumerableExceptionHandlerInterfaceType = typeof(IEnumerable<>).MakeGenericType(exceptionHandlerInterfaceType);
        Array resultArray = CreateArraysOutOfResolvedTypeAndEnumerableInterfaceTypes(type, resolvedType, serviceFactory, enumerableExceptionHandlerInterfaceType);

        return resultArray;
    }

    private static object ResolveRequestExceptionAction(IKernel k, Type type, Type service, object resolvedType, Type[] genericArguments)
    {
        if (service == null
        || genericArguments == null
        || !service.IsInterface
        || !service.IsGenericType
        || !service.IsConstructedGenericType
        || !(service.GetGenericTypeDefinition()
        ?.IsAssignableTo(typeof(IRequestExceptionAction<,>)) ?? false)
        || genericArguments.Length != 2)
        {
            return resolvedType;
        }

        var serviceFactory = k.Resolve<ServiceFactory>();
        var baseRequestType = genericArguments[0].BaseType;
        var exceptionType = genericArguments[1];

        // Check if the base request type is valid
        if (baseRequestType == null
        || !baseRequestType.IsClass
        || baseRequestType == typeof(object)
        || ((!baseRequestType.GetInterfaces()
            ?.Any(i => i.IsAssignableFrom(typeof(IRequest<>)))) ?? true))
        {
            return resolvedType;
        }

        var exceptionHandlerInterfaceType = typeof(IRequestExceptionAction<,>).MakeGenericType(baseRequestType, exceptionType);
        var enumerableExceptionHandlerInterfaceType = typeof(IEnumerable<>).MakeGenericType(exceptionHandlerInterfaceType);
        Array resultArray = CreateArraysOutOfResolvedTypeAndEnumerableInterfaceTypes(type, resolvedType, serviceFactory, enumerableExceptionHandlerInterfaceType);

        return resultArray;
    }

    private static Array CreateArraysOutOfResolvedTypeAndEnumerableInterfaceTypes(Type type, object resolvedType, ServiceFactory serviceFactory, Type enumerableExceptionHandlerInterfaceType)
    {
        var firstArray = serviceFactory.Invoke(enumerableExceptionHandlerInterfaceType) as Array;
        Debug.Assert(firstArray != null, $"Array '{nameof(firstArray)}' should not be null because this method calls ResolveAll when a {typeof(IEnumerable<>).FullName} " +
            $"is passed as argument in argument named '{nameof(type)}'");

        var secondArray = resolvedType is Array ? resolvedType as Array : new[] { resolvedType };
        Debug.Assert(secondArray != null, $"Array '{nameof(secondArray)}' should not be null because '{nameof(resolvedType)}' is an array or created as an array");

        var resultArray = Array.CreateInstance(typeof(object), firstArray.Length + secondArray.Length);
        Array.Copy(firstArray, resultArray, firstArray.Length);
        Array.Copy(secondArray, 0, resultArray, firstArray.Length, secondArray.Length);
        return resultArray;
    }
}