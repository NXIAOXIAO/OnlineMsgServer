using System.Net;
using OnlineMsgServer.Common;
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace OnlineMsgServer.Core
{
    class WsService : WebSocketBehavior
    {
        private IPEndPoint iPEndPoint = new(IPAddress.Any, 0);
        protected override async void OnMessage(MessageEventArgs e)
        {
            try
            {
                Common.Log.Debug(ID + " " + Context.UserEndPoint.ToString() + ":" + e.Data);
                //从base64字符串解密
                string decryptString = RsaService.Decrypt(e.Data);
                //json 反序列化
                Message? message = Message.JsonStringParse(decryptString);
                if (message != null)
                {
                    await message.HandlerAndMeasure(ID, Sessions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing message: " + ex.Message);
            }
        }

        protected override void OnOpen()
        {
            iPEndPoint = Context.UserEndPoint;
            UserService.AddUserConnect(ID);
            Common.Log.Debug(ID + " " + iPEndPoint.ToString() + " Conection Open");
            //连接时回复公钥，不加密
            Message response = new()
            {
                Type = "PublicKey",
                Data = RsaService.GetRsaPublickKey(),
            };
            string jsonString = response.ToJsonString();
            Send(jsonString);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            UserService.RemoveUserConnectByID(ID);
            Common.Log.Debug(this.ID + " " + this.iPEndPoint.ToString() + " Conection Close" + e.Reason);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            UserService.RemoveUserConnectByID(ID);
            Common.Log.Debug(this.ID + " " + this.iPEndPoint.ToString() + " Conection Error Close" + e.Message);
        }
    }
}