using System.Net;

namespace OnlineMsgServer.Common
{
    public class User(string ID)
    {
        /// <summary>
        /// ws连接生成的唯一uuid
        /// </summary>
        public string ID { get; set; } = ID;

        /// <summary>
        /// 用户名，在客户端随意指定
        /// </summary>
        public string? Name { get; set; }


        /// <summary>
        /// 用户公钥 用于消息加密发送给用户
        /// </summary>
        public string? PublicKey { get; set; }
    }
}