using OnlineMsgServer.Core;
using WebSocketSharp.Server;

namespace OnlineMsgServer.Common
{
    class ForwardMessage : Message
    {
        public ForwardMessage()
        {
            Type = "forward";
            //收到客户端消息，并执行转发
        }
        public override Task Handler(string wsid, WebSocketSessionManager Sessions)
        {
            return Task.Run(() =>
            {
                try
                {
                    string forwardPublickKey = Key!;
                    string forwardData = Data!.GetString();
                    string fromPublicKey = UserService.GetUserPublicKeyByID(wsid)!;

                    Message response = new()
                    {
                        Type = "forward",
                        Data = forwardData,
                        Key = fromPublicKey,
                    };

                    string jsonString = response.ToJsonString();
                    string encryptString = RsaService.EncryptForClient(forwardPublickKey, jsonString);

                    List<User> userList = UserService.GetUserListByPublicKey(forwardPublickKey);

                    foreach (IWebSocketSession session in Sessions.Sessions)
                    {
                        if (userList.Exists(u => u.ID == session.ID))
                        {
                            session.Context.WebSocket.Send(encryptString);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }
    }
}