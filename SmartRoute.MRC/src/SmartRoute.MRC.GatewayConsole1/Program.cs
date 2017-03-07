using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace SmartRoute.MRC.GatewayConsole1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            INode node = NodeFactory.Default;
            node.Loger.Type = LogType.ALL;
            node.AddLogHandler(new SmartRoute.ConsoleLogHandler(LogType.ALL));
            node.Open();
            MRC.MCRFactory.Gateway.Open();
            System.Threading.ThreadPool.QueueUserWorkItem(OnTest);
            System.Threading.Thread.Sleep(-1);
        }

        private static void OnTest(object state)
        {
            System.Threading.Thread.Sleep(10000);
            UserToken token = new UserToken("ken");
            token.Register();
            token.Receive = OnUserReceive;
            while (true)
            {
                for (int i = 0; i < 1000; i++)
                    token.Send("henry", DateTime.Now);
                System.Threading.Thread.Sleep(200);
            }
        }

        private static void OnUserReceive(UserToken token, Protocol.UserMessage e)
        {
            Console.WriteLine("receive message from {0} {1}", e.Sender, e.Data);
        }
    }
}
