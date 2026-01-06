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
            services.AddApiControllers();
            services.AddSwagger();
            services.DiscoverServices();

            return services;
        }

        internal void AddApiControllers()
        {
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
        }

        internal void AddSwagger()
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}