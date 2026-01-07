using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Mapster;
using MapsterMapper;
using SeatBooking.Application.BackgroundServices;
using SeatBooking.Core.DependencyInjections.Extensions;

namespace SeatBooking.Application.Extensions;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddMapster();
            services.AddHostedService<ReservationExpirationBackgroundService>();
            services.DiscoverServices();

            return services;
        }

        internal void AddMapster()
        {
            var globalSettings = TypeAdapterConfig.GlobalSettings;

            globalSettings.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(globalSettings);
            services.AddScoped<IMapper, ServiceMapper>();
        }
    }
}