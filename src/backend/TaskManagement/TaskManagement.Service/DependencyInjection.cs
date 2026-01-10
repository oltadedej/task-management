using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Service.Behaviors;
using TaskManagement.Service.Mappings;

namespace TaskManagement.Service;

/// <summary>
/// Dependency injection configuration for the Service layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds service layer dependencies to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddServiceLayer(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register validation behavior
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Register AutoMapper
        services.AddAutoMapper(typeof(TaskMappingProfile));

        return services;
    }
}

