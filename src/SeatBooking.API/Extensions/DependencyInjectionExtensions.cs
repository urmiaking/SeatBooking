using Microsoft.Extensions.DependencyInjection;
using SeatBooking.Core.DependencyInjections.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SeatBooking.API.Extensions;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApis()
        {
            services.AddApiControllers()
                .AddSwagger()
                .DiscoverServices()
                .AddRealtime();

            return services;
        }

        internal IServiceCollection AddApiControllers()
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

        internal IServiceCollection AddSwagger()
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        internal IServiceCollection AddRealtime()
        {
            services.AddSignalR();

            return services;
        }
    }
}