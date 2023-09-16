using System.Text.Json;
using System.Text.Json.Serialization;
using Ilmhub.Judge.Sdk.Dtos;

namespace Ilmhub.Judge.Sdk.Json;

public class PingResponseConverter : JsonConverter<PingResponse>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(PingResponse).IsAssignableFrom(typeToConvert);
    }

    public override PingResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new PingResponse();

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of an object.");
        }

        string propertyName = string.Empty;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
                propertyName = reader.GetString();
            else if (reader.TokenType == JsonTokenType.StartObject && propertyName == "data")
            {
                var pingData = JsonSerializer.Deserialize<PingResponseData>(ref reader, options);

                result.Data = pingData;
            }
            else if (reader.TokenType == JsonTokenType.String && propertyName == "data")
            {
                result.Data = null;
                result.ErrorMessage = reader.GetString();
            }
            else if (propertyName == "err")
                result.Err = reader.GetString();
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, PingResponse value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
