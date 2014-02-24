using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Microsoft.LogHub.SignalrStartup))]
namespace Microsoft.LogHub
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