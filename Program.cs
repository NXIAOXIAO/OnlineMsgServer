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
        static async Task Main(string[] args)
        {
            try
            {
                await MainLoop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static async Task MainLoop()
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

            bool loopFlag = true;
            while (loopFlag)
            {
#if DEBUG
                Console.WriteLine("输入exit退出程序");
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
#endif
                await Task.Delay(5000);// 每5秒检查一次
            }
            wssv.Stop();
        }
    }
}