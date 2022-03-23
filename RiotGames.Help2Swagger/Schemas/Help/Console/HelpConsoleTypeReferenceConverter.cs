using System.Text.Json;
using System.Text.Json.Serialization;

namespace RiotGames.Help;

public class HelpConsoleTypeReferenceConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        // If the token is a string, return string
        if (reader.TokenType == JsonTokenType.String) return reader.GetString()!;

        var type = JsonSerializer.Deserialize<Dictionary<string, HelpConsoleType>>(ref reader, options);

        return type!;
    }

    public override void Write(Utf8JsonWriter writer, object value,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}