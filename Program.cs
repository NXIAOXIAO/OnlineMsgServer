using System.Net;
using System.Net.Sockets;
using System.Text;
using OnlineMsgServer.Common;
using OnlineMsgServer.Core;
using WebSocketSharp.Server;

namespace OnlineMsgServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MainLoop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void MainLoop()
        {

            //初始化RSA
            RsaService.LoadRsaPkey();
            //设置端口 考虑到需要容器化，还是指定一个端口比较好 随机选择的13173
            int ListenPort = 13173;
            //开启ws监听
            var wssv = new WebSocketServer(ListenPort, false);
            wssv.AddWebSocketService<WsService>("/");
            wssv.Start();
            Console.WriteLine("已开启ws监听, 端口: " + ListenPort);
            Console.WriteLine("输入exit退出程序");

            bool loopFlag = true;
            while (loopFlag)
            {
                string input = Console.ReadLine() ?? "";
                switch (input.Trim())
                {
                    case "exit":
                        loopFlag = false;
                        break;
                    case "port":
                        Console.WriteLine("服务器开放端口为" + ListenPort);
                        break;
                    default:
                        break;
                }
            }
            wssv.Stop();
        }
    }
}