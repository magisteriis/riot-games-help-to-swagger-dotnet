using System.Text.Json;
using System.Text.Json.Serialization;

namespace RiotGames.Help;

public class HelpConsoleTypeReferenceConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        // If the token is a string, return null
        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString()!;
        }

        // Skip over the object and property name
        //reader.Read(); // Object
        //reader.Read(); // Property name

        // Read the tags array
        var tags = JsonSerializer.Deserialize<Dictionary<string, HelpConsoleTypeSchema>>(ref reader, options);

        //reader.Read(); // Object

        return tags;
    }

    public override void Write(Utf8JsonWriter writer, object value,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}