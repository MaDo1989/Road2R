using Microsoft.Owin;
using Owin;
using System;
using System.Threading;
using System.Threading.Tasks;

[assembly: OwinStartup("ProductionConfiguration", typeof(Signalr_Startup_01))]

public class Signalr_Startup_01
{
    public void Configuration(IAppBuilder app)
    {
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        app.MapSignalR();
    }

}
