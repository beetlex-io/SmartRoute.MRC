using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRoute.MRC
{
	public interface IGateway : IDisposable
	{
		INode Node { get; set; }

		Protocol.OperationStatus Register(UserToken userToken);

		Protocol.OperationStatus UnRegister(string username);

		void SendMessage(string receivers, string sender, object message);

		void Open();
	}
}
