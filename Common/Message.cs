using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSocketSharp.Server;

namespace OnlineMsgServer.Common
{
    class Message
    {

        public string Type { get; set; }
        /// <summary>
        /// 转发的目标
        /// </summary>
        public string? Key { get; set; }
        public dynamic? Data { get; set; }
        public Message()
        {
            Type = "";
        }

        public static readonly JsonSerializerOptions options = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip, //允许注释
            AllowTrailingCommas = true,//允许尾随逗号
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // 忽略 null 值
            WriteIndented = true, // 美化输出
            //PropertyNameCaseInsensitive = true,//属性名忽略大小写
            Converters = { new MessageConverter() },
        };

        public static Message? JsonStringParse(string jsonString)
        {
            try
            {
                return JsonSerializer.Deserialize<Message>(jsonString, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public string ToJsonString()
        {
            return JsonSerializer.Serialize(this, options);
        }

        /// <summary>
        /// 指令处理逻辑(包括加密及发送过程)
        /// </summary>
        /// <returns>将耗时任务交给Task以不阻塞单个连接的多个请求</returns>
        public virtual Task Handler(string wsid, WebSocketSessionManager Sessions)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 增加计时逻辑
        /// </summary>
        public async Task HandlerAndMeasure(string wsid, WebSocketSessionManager Sessions)
        {
            Stopwatch stopWatch = new();
            stopWatch.Start();
            await Handler(wsid, Sessions);
            stopWatch.Stop();
            Log.Debug($"处理{GetType()}耗时：{stopWatch.ElapsedMilliseconds}ms");
        }

    }
}