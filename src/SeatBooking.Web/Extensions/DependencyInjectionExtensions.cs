using SeatBooking.Core.DependencyInjections.Extensions;
using SeatBooking.Core.Settings;

namespace SeatBooking.Web.Extensions;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddServer(IConfiguration configuration)
        {
            services.AddPages()
                .AddServices()
                .AddSettings(configuration);

            return services;
        }

        internal IServiceCollection AddPages()
        {
            services.AddRazorPages();
            return services;
        }

        internal IServiceCollection AddServices()
        {
            services.DiscoverServices();

            return services;
        }

        internal void AddSettings(IConfiguration configuration)
        {
            services.Configure<SeatBookingSettings>(configuration.GetSection(nameof(SeatBookingSettings)));
        }   
    }
}