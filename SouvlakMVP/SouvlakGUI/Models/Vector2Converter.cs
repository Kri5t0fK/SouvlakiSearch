using System.Numerics;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SouvlakGUI.Models;


/// <summary>
/// Provides custom JSON serialization and deserialization for Vector2 objects using the System.Text.Json namespace.
/// </summary>
public class Vector2Converter : JsonConverter<Vector2>
{
    /// <summary>
    /// Reads JSON and converts it into a Vector2 object.
    /// </summary>
    /// <param name="reader">The reader used to parse the JSON data.</param>
    /// <param name="typeToConvert">The type of object to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>A Vector2 object deserialized from the JSON data.</returns>
    public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"Unexpected token type '{reader.TokenType}'");
        }

        float x = 0, y = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new Vector2(x, y);
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException($"Unexpected token type '{reader.TokenType}'");
            }

            string propertyName = reader.GetString();

            if (!reader.Read())
            {
                throw new JsonException($"Unexpected end of JSON object");
            }

            switch (propertyName)
            {
                case "X":
                    x = reader.GetSingle();
                    break;
                case "Y":
                    y = reader.GetSingle();
                    break;
                default:
                    throw new JsonException($"Unknown property '{propertyName}'");
            }
        }

        throw new JsonException("Unexpected end of JSON input");
    }

    /// <summary>
    /// Writes a Vector2 object to JSON.
    /// </summary>
    /// <param name="writer">The writer used to write the JSON data.</param>
    /// <param name="value">The Vector2 object to serialize.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteEndObject();
    }
}
