using SeatBooking.Application.Extensions;
using SeatBooking.Infrastructure.Extensions;

namespace SeatBooking.Web.Extensions;

public static class WebHostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services
            .AddServer(configuration)
            .AddApplication()
            .AddInfrastructure(configuration);

        return builder.Build();
    }
}