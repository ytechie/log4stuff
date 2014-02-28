using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Microsoft.LogHub
{
    public class LogMessageHub : Hub
    {
        public override Task OnConnected()
        {
            RegisterConnection();

            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            RegisterConnection();

            return base.OnReconnected();
        }

        private void RegisterConnection()
        {
            var applicationId = Context.QueryString["applicationId"];
            Groups.Add(Context.ConnectionId, applicationId);
        }

        public override Task OnDisconnected()
        {
            var applicationId = Context.QueryString["applicationId"];
            Groups.Remove(Context.ConnectionId, applicationId);
            
            return base.OnDisconnected();
        }

        public void SendMsg(string applicationId, string logMessageJson)

        {
            Clients.Client(applicationId).newLogMessage(logMessageJson);
        }
    }
}