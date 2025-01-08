using OnlineMsgServer.Core;
using WebSocketSharp.Server;

namespace OnlineMsgServer.Common
{
    class BroadcastMessage : Message
    {
        public BroadcastMessage()
        {
            Type = "broadcast";
            //收到客户端消息，并执行广播
        }
        public override Task Handler(string wsid, WebSocketSessionManager Sessions)
        {
            return Task.Run(() =>
            {
                try
                {
                    string broadcastData = Data!.GetString();

                    Message response = new()
                    {
                        Type = "broadcast",
                        Data = broadcastData,
                        Key = UserService.GetUserNameByID(wsid),
                    };

                    foreach (IWebSocketSession session in Sessions.Sessions)
                    {
                        if (session.ID != wsid)//不用发给自己
                        {
                            string? publicKey = UserService.GetUserPublicKeyByID(session.ID);
                            if (publicKey != null)
                            {
                                string jsonString = response.ToJsonString();
                                string encryptString = RsaService.EncryptForClient(publicKey, jsonString);
                                session.Context.WebSocket.Send(encryptString);
                            }
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