using Log4stuff.Web;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof (SignalrStartup))]

namespace Log4stuff.Web
{
    public class SignalrStartup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}