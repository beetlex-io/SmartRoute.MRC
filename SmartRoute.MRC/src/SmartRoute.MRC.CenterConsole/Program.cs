using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRoute.MRC.CenterConsole
{
	public class Program
	{
		public static void Main(string[] args)
		{
			INode node = NodeFactory.Default;
			node.Loger.Type = LogType.ALL;
			node.AddLogHandler(new SmartRoute.ConsoleLogHandler(LogType.ALL));
			node.Open();
			MRC.MCRFactory.Center.Open();
			System.Threading.Thread.Sleep(-1);
		}
	}
}
