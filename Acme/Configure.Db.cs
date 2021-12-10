using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;

[assembly: HostingStartup(typeof(Acme.ConfigureDb))]

namespace Acme
{
    public class ConfigureDb : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder) => builder
            .ConfigureServices((context, services) => {
                services.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(
                    context.Configuration.GetConnectionString("DefaultConnection")
                    ?? "bookings.sqlite",
                    SqliteDialect.Provider));
            })
            .ConfigureAppHost(afterConfigure:appHost => {
                appHost.GetPlugin<SharpPagesFeature>()?.ScriptMethods.Add(new DbScriptsAsync());

                using var db = appHost.Resolve<IDbConnectionFactory>().Open();
                db.CreateTableIfNotExists<Booking>();
            });
    }
}
