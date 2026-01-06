using SeatBooking.Infrastructure;
using SeatBooking.Web.Extensions;

try
{
    Console.WriteLine("Starting up.");

    var builder = WebApplication.CreateBuilder(args);

    var app = builder.ConfigureServices();

    app.ApplyDatabaseMigrations<AppDbContext>();
    await app.SeedDatabaseAsync();

    app.ConfigurePipeline();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("An error occured:");
    Console.WriteLine(ex.Message);
}
finally
{
    Console.ResetColor();
    Console.WriteLine("Shutting down completed.");
}