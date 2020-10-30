using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace Acme
{
    public class ConfigureDb : IConfigureServices, IConfigureAppHost
    {
        IConfiguration Configuration { get; }
        public ConfigureDb(IConfiguration configuration) => Configuration = configuration;

        public void Configure(IServiceCollection services)
        {
            services.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(
                Configuration.GetConnectionString("DefaultConnection") 
                    ?? "bookings.sqlite",
                SqliteDialect.Provider));
        }

        public void Configure(IAppHost appHost)
        {
            appHost.GetPlugin<SharpPagesFeature>()?.ScriptMethods.Add(new DbScriptsAsync());

            using var db = appHost.Resolve<IDbConnectionFactory>().Open();
            db.CreateTableIfNotExists<Booking>();
        }
    }    
}
