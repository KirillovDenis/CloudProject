using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(Scheduler.Startup))]

namespace Scheduler
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/mysignalr",map => {
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {

                };
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}
