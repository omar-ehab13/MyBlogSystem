using BlogSystem.Domain.Repositories;
using BlogSystem.Infrastructure.Data;
using BlogSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogSystem.Infrastructure
{
    public static class InfrastructureLayerConfigurations
    {
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BlogSystemDbContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            //services.AddDbContext<BlogSystemDbContext>(opt =>
            //    opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
