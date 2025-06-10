using BlogSystem.Application.Behaviors;
using BlogSystem.Application.Interfaces;
using BlogSystem.Application.Mappings;
using BlogSystem.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BlogSystem.Application
{
    public static class ApplicationLayerConfigurations
    {
        public static void ConfigureMappingService(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(BlogMappingProfile).Assembly);


            services.AddScoped<IMappingService, MappingService>();
        }

        public static void ConfigureMediatR(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(typeof(ApplicationLayerConfigurations).Assembly));
        }

        public static void ConfigureFluentValidation(this IServiceCollection services)
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddValidatorsFromAssembly(typeof(ApplicationLayerConfigurations).Assembly);
        }
    }
}
