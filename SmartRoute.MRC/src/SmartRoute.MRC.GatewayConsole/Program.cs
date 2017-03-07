using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRoute.MRC.GatewayConsole
{
	public class Program
	{
		static long mReceives;

		static long mLastReceives;

		public static void Main(string[] args)
		{
			INode node = NodeFactory.Default;
			node.Loger.Type = LogType.ALL;
			node.AddLogHandler(new SmartRoute.ConsoleLogHandler(LogType.ALL));
			node.Open();
			MRC.MCRFactory.Gateway.Open();
			System.Threading.ThreadPool.QueueUserWorkItem(OnTest);
			while (true)
			{
				Console.WriteLine("{0}/{1}", mReceives - mLastReceives, mReceives);
				mLastReceives = mReceives;
				System.Threading.Thread.Sleep(1000);
			}
		}

		private static void OnTest(object state)
		{
			System.Threading.Thread.Sleep(10000);
			UserToken token = new UserToken("henry");
			token.Register();
			token.Receive = OnUserReceive;

		}

		private static void OnUserReceive(UserToken token, Protocol.UserMessage e)
		{
			//Console.WriteLine("receive message from {0} {1}", e.Sender, e.Data);
			System.Threading.Interlocked.Increment(ref mReceives);
		}
	}
}
