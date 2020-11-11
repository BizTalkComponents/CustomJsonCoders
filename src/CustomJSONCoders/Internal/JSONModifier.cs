using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace BizTalkComponents.PipelineComponents.CustomJsonCoders.Internal
{
    internal static class JSONModifier
    {
        internal static Stream ConvertToJArray(JsonTextReader reader, bool rootArray = true)
        {
            var ms = new MemoryStream();
            var writer = new JsonTextWriter(new StreamWriter(ms, Encoding.UTF8));
            while (reader.Read())
            {
                var tokenType = reader.TokenType;
                var depth = reader.Depth;
                if (rootArray)
                {
                    if (depth == 0 & tokenType == JsonToken.StartObject)
                    {
                        writer.WriteStartArray();
                    }
                    else if (depth==0 & tokenType ==JsonToken.String)
                    {
                        writer.WriteStartArray();
                        writer.WriteEndArray();
                        break;
                    }
                    else if (depth == 1 &
                        (tokenType == JsonToken.StartArray
                        | tokenType == JsonToken.EndArray
                        | tokenType == JsonToken.PropertyName))
                    {

                    }
                    else if (depth == 0 & tokenType == JsonToken.EndObject)
                    {
                        writer.WriteEndArray();
                    }
                    else
                        WriteJson(reader, writer);
                }
            }
            reader.Close();
            writer.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
        private static void WriteJson(JsonTextReader reader, JsonTextWriter writer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.None:
                    break;
                case JsonToken.StartObject:
                    writer.WriteStartObject();
                    break;
                case JsonToken.StartArray:
                    writer.WriteStartArray();
                    break;
                case JsonToken.StartConstructor:
                    break;
                case JsonToken.PropertyName:
                    writer.WritePropertyName((string)reader.Value);
                    break;
                case JsonToken.Comment:
                    writer.WriteComment((string)reader.Value);
                    break;
                case JsonToken.Raw:
                    break;
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    writer.WriteValue(reader.Value);
                    break;
                case JsonToken.Null:
                    writer.WriteNull();
                    break;
                case JsonToken.Undefined:
                    writer.WriteUndefined();
                    break;
                case JsonToken.EndObject:
                    writer.WriteEndObject();
                    break;
                case JsonToken.EndArray:
                    writer.WriteEndArray();
                    break;
                case JsonToken.EndConstructor:
                    break;
                default:
                    break;
            }
        }
    }

}
