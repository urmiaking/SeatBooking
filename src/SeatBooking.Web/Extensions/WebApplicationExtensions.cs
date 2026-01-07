using Microsoft.EntityFrameworkCore;
using SeatBooking.API.Hubs;
using SeatBooking.Application.Abstractions;

namespace SeatBooking.Web.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        internal void ConfigurePipeline()
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.MapStaticAssets();
            app.MapRazorPages()
                .WithStaticAssets();

            app.MapControllers();

            app.MapHub<SeatBookingHub>("/hubs/seat-booking");
        }

        internal void ApplyDatabaseMigrations<TContext>()
            where TContext : DbContext
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            var pendingMigrations = context.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Count > 0)
            {
                context.Database.Migrate();
            }
        }

        internal async Task SeedDatabaseAsync(CancellationToken cancellationToken = default)
        {
            using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

            foreach (var service in scope.ServiceProvider.GetServices<IDbSeeder>())
                await service.SeedAsync(cancellationToken);
        }
    }
}