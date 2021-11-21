using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(FurDepot.Controllers.ChatHub))]
namespace FurDepot.Controllers {
	public class ChatHub : Hub {
		static ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();
		public void Send(string name, string message) {
			Clients.All.broadcastMessage(name, message);
		}

		public void Configuration(IAppBuilder app) {
			// Any connection or hub wire up and configuration should go here
			app.MapSignalR();
		}
		public void SendToSpecific(string name, string message, string to) {
			Clients.Caller.broadcastMessage(name, message);
			Clients.Client(dic[to]).broadcastMessage(name, message);
		}

		public void Notify(string name, string id) {
			if (dic.ContainsKey(name)) {
				Clients.Caller.differentName();
			}
			else {
				dic.TryAdd(name, id);
				foreach (KeyValuePair<String, String> entry in dic) {
					Clients.Caller.online(entry.Key);
				}
				Clients.Others.enters(name);
			}
		}

		public override Task OnDisconnected(bool StopCalled) {
			var name = dic.FirstOrDefault(x => x.Value == Context.ConnectionId.ToString());
			string s;
			dic.TryRemove(name.Key, out s);
			return Clients.All.disconnected(name.Key);
		}
	}
}