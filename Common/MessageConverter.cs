using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnlineMsgServer.Common
{
    /// <summary>
    /// 实现依据instruct的值决定反序列化的对象类型
    /// </summary>
    class MessageConverter : JsonConverter<Message>
    {
        public override Message Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement root = doc.RootElement;
            if (root.TryGetProperty("type", out JsonElement typeProperty))
            {
                string? instruct = typeProperty.GetString();
                Message? message = instruct switch
                {
                    //实现新指令需在这里添加反序列化类型，限定为客户端发过来的指令类型
                    // "test" => JsonSerializer.Deserialize<Message>(root.GetRawText(), options),
                    "publickey" => JsonSerializer.Deserialize<PublicKeyMessage>(root.GetRawText(), options),
                    "forward" => JsonSerializer.Deserialize<ForwardMessage>(root.GetRawText(), options),
                    "broadcast" => JsonSerializer.Deserialize<BroadcastMessage>(root.GetRawText(), options),
                    _ => null
                };
                return message ?? throw new JsonException($"{instruct} 反序列化失败");
            }
            else
            {
                throw new JsonException("instruct property not found");
            }
        }

        public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions options)
        {
            //JsonSerializer.Serialize(writer, (object)value, options); 这个在Data是对象时导致了无限递归调用
            writer.WriteStartObject();
            writer.WriteString("type", value.Type);
            if (value.Key != null)
            {
                writer.WriteString("key", value.Key);
            }
            if (value.Data != null)
            {
                writer.WritePropertyName("data");
                JsonSerializer.Serialize(writer, value.Data, value.Data.GetType(), options);
            }
            writer.WriteEndObject();
        }
    }
}