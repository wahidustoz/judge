using System.Text.Json;
using System.Text.Json.Serialization;
using Ilmhub.Judge.Sdk.Dtos;

namespace Ilmhub.Judge.Sdk.Json;

public class JudgeResponseConverter : JsonConverter<JudgeResponse>
{
    public override bool CanConvert(Type typeToConvert)
        => typeof(JudgeResponse).IsAssignableFrom(typeToConvert);

    public override void Write(Utf8JsonWriter writer, JudgeResponse value, JsonSerializerOptions options)
        => throw new NotImplementedException();

    public override JudgeResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new JudgeResponse();

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of an object.");
        }

        string currentProperty = string.Empty;
        while (reader.Read())
        {
            if (TokenIsPropertyName(reader))
                currentProperty = reader.GetString();
            else if (TokenIsErrorMessage(reader, currentProperty))
                result.ErrorMessage = reader.GetString();
            else if (TokenIsTestCaseArray(reader, currentProperty))
            {
                var testCases = JsonSerializer.Deserialize<IEnumerable<TestCaseResponseDto>>(ref reader, options);
                result.TestCases = testCases;
            }
            else if (TokenIsCompileErrorData(reader, currentProperty, result.ErrorMessage))
            {
                result.TestCases = Enumerable.Empty<TestCaseResponseDto>();
                result.CompileError = new CompileErrorDto();
                var dataString = reader.GetString();
                if (TryGetCompilerRuntimeErrorInfoString(dataString, out var infoString))
                    result.CompileError = JsonSerializer.Deserialize<CompileErrorDto>(infoString, options);
                else
                    result.ErrorMessage = dataString;
            }
        }

        return result;
    }

    private bool TokenIsPropertyName(Utf8JsonReader reader) => reader.TokenType is JsonTokenType.PropertyName;
    private bool TokenIsErrorMessage(Utf8JsonReader reader, string currentProperty)
        => reader.TokenType is JsonTokenType.String && currentProperty.Equals("err");
    private bool TokenIsTestCaseArray(Utf8JsonReader reader, string currentProperty)
        => reader.TokenType is JsonTokenType.StartArray && currentProperty.Equals("data");
    private bool TokenIsCompileErrorData(Utf8JsonReader reader, string currentProperty, string errorMessage)
        => reader.TokenType is JsonTokenType.String
            && currentProperty.Equals("data")
            && errorMessage.Equals("CompileError", StringComparison.OrdinalIgnoreCase);
    private bool TryGetCompilerRuntimeErrorInfoString(string errorString, out string infoString)
    {
        if (errorString.IndexOf("info:") != -1)
        {
            infoString = errorString[(errorString.IndexOf("info:") + 5)..];     // using range operator for substring
            return true;
        }

        infoString = string.Empty;
        return false;
    }
}
