using OnlineMsgServer.Core;
using WebSocketSharp.Server;

namespace OnlineMsgServer.Common
{
    class PublicKeyMessage : Message
    {
        public PublicKeyMessage()
        {
            Type = "publickey";
            //收到客户端公钥，添加到用户列表中，并返回自己的公钥
        }
        public override Task Handler(string wsid, WebSocketSessionManager Sessions)
        {
            return Task.Run(() =>
            {
                try
                {
                    string userPublicKey = Data!.GetString();
                    string userName = Key ?? "Anthony";
                    UserService.UserLogin(wsid, userPublicKey, userName);
                    //这里不返回自己的公钥，改成服务端在连接时回复公钥
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }
    }
}