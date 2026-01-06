using SeatBooking.Core.DependencyInjections.Extensions;
using SeatBooking.Core.Settings;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SeatBooking.Web.Extensions;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddServer(IConfiguration configuration)
        {
            services
                .AddApis()
                .AddPages()
                .AddServices()
                .AddSwagger()
                .AddSettings(configuration);

            return services;
        }

        internal IServiceCollection AddApis()
        {
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

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

        internal IServiceCollection AddSwagger()
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        internal IServiceCollection AddSettings(IConfiguration configuration)
        {
            services.Configure<SeatBookingSettings>(configuration.GetSection(nameof(SeatBookingSettings)));
            return services;
        }   
    }
}