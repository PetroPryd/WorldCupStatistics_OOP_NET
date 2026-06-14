using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Serialization
{
    internal static class JsonOptionsFactory
    {
        public static readonly JsonSerializerOptions Default = CreateOptions();

        private static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };
            options.Converters.Add(new NullSafeIntConverter());
            return options;
        }
    }

    internal sealed class NullSafeIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return 0;
                case JsonTokenType.Number:
                    return reader.TryGetInt32(out var n) ? n : 0;
                case JsonTokenType.String:
                    return int.TryParse(reader.GetString(), out var s) ? s : 0;
                default:
                    return 0;
            }
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value);
    }
}
