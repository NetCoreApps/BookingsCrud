using Funq;
using ServiceStack;
using Acme.ServiceInterface;
using ServiceStack.Admin;
using ServiceStack.Data;

[assembly: HostingStartup(typeof(Acme.AppHost))]

namespace Acme;
public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services => {
            // Configure ASP.NET Core IOC Dependencies
        })
        .Configure(app => {
            // Configure ASP.NET Core App
            if (!HasInit)
                app.UseServiceStack(new AppHost());
        });

    public AppHost() : base("Acme", typeof(MyServices).Assembly) {}

    public override void Configure(Container container)
    {
        // Configure ServiceStack only IOC, Config & Plugins
        SetConfig(new HostConfig
        {
            DebugMode = AppSettings.Get(nameof(HostConfig.DebugMode), false)
        });
            
        Plugins.Add(new AutoQueryFeature {
            MaxLimit = 1000
        });
            
        Plugins.Add(new AdminUsersFeature());

        container.AddSingleton<ICrudEvents>(c =>
            new OrmLiteCrudEvents(c.Resolve<IDbConnectionFactory>()));
        container.Resolve<ICrudEvents>().InitSchema();
    }
}
