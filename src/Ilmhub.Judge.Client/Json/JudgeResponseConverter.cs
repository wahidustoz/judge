using System.Text.Json;
using System.Text.Json.Serialization;
using Ilmhub.Judge.Client.Dtos;

namespace Ilmhub.Judge.Client.Json;

public class JudgeResponseConverter : JsonConverter<JudgeResponse>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(JudgeResponse).IsAssignableFrom(typeToConvert);
    }

    public override JudgeResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new JudgeResponse();

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of an object.");
        }

        string propertyName = string.Empty;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
                propertyName = reader.GetString();
            else if (reader.TokenType == JsonTokenType.String && propertyName == "err")
                result.ErrorMessage = reader.GetString();
            else if (reader.TokenType == JsonTokenType.String && propertyName == "data")
            {
                result.TestCases = Enumerable.Empty<TestCaseResponseDto>();
                result.ErrorMessage = $"{result.ErrorMessage}\n{reader.GetString()}";
            }
            else if (reader.TokenType == JsonTokenType.StartArray && propertyName == "data")
            {
                var testCases = JsonSerializer.Deserialize<IEnumerable<TestCaseResponseDto>>(ref reader, options);
                result.TestCases = testCases;
            }
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, JudgeResponse value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
