using System.Reflection;
using GiapTech.BlouseHiding.Application.Common.Behaviours;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        builder.Services.AddMapster();

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Transient;
            options.PipelineBehaviors =
            [
                typeof(UnhandledExceptionBehaviour<,>),
                typeof(AuthorizationBehaviour<,>),
                typeof(ValidationBehaviour<,>),
                typeof(LoggingBehaviour<,>),
                typeof(PerformanceBehaviour<,>),
            ];
        });
    }
}
