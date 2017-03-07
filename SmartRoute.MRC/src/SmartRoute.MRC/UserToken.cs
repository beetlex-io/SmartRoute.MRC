using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRoute.MRC
{
	public class UserToken : IDisposable
	{
		public UserToken(string name, IGateway gateway = null)
		{
			Name = name;
			if (gateway == null)
				gateway = MCRFactory.Gateway;
			Gateway = gateway;

		}

		public Protocol.OperationStatus Register()
		{
			return Gateway.Register(this);
		}

		public IGateway Gateway { get; set; }

		public string Name { get; set; }

		public object Context { get; set; }

		public Action<UserToken, Protocol.UserMessage> Receive { get; set; }

		public void Dispose()
		{
			Gateway.UnRegister(Name);
		}

		public void Send(string receivers, object data)
		{
			Gateway.SendMessage(receivers, Name, data);
		}
	}
}
