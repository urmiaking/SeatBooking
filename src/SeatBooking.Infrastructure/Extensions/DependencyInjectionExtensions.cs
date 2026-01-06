using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeatBooking.Core.DependencyInjections.Extensions;

namespace SeatBooking.Infrastructure.Extensions;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.AddStorage(configuration);
            services.DiscoverServices();

            return services;
        }

        internal void AddStorage(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default");

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Default connection string is not available");

            services.AddSqlServer<AppDbContext>(connectionString);
        }
    }
}