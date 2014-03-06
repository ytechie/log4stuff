using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Log4stuff.Web
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
            string applicationId = Context.QueryString["applicationId"];
            Groups.Add(Context.ConnectionId, applicationId);
        }

        public override Task OnDisconnected()
        {
            string applicationId = Context.QueryString["applicationId"];
            Groups.Remove(Context.ConnectionId, applicationId);

            return base.OnDisconnected();
        }

        public void SendMsg(string applicationId, string logMessageJson)

        {
            Clients.Client(applicationId).newLogMessage(logMessageJson);
        }
    }
}