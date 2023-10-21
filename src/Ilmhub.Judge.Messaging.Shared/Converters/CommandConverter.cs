using System.Text.Json;
using System.Text.Json.Serialization;
using Ilmhub.Judge.Messaging.Shared.Commands;
using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging.Shared.Converters;

public class CommandConverter : JsonConverter<ICommand>
{
    private const string DESCRIMINATOR = "$command";
    private readonly Type[] types = new Type[]
    {
        typeof(JudgeCommand),
        typeof(RunCommand)
    };

    public override bool CanConvert(Type type)
        => typeof(ICommand).IsAssignableFrom(type);

    public override ICommand Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject)
            throw new JsonException();

        using var jsonDocument = JsonDocument.ParseValue(ref reader);

        if (false == jsonDocument.RootElement.TryGetProperty(DESCRIMINATOR, out var typeProperty))
            throw new JsonException();

        var type = types.FirstOrDefault(x => x.Name == typeProperty.GetString())
            ?? throw new JsonException();

        var jsonObject = jsonDocument.RootElement.GetRawText();
        var result = (ICommand)JsonSerializer.Deserialize(jsonObject, type);

        return result;
    }

    public override void Write(Utf8JsonWriter writer, ICommand value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        using (JsonDocument document = JsonDocument.Parse(JsonSerializer.Serialize(value, value.GetType())))
        {
            writer.WritePropertyName(DESCRIMINATOR);
            writer.WriteStringValue(value.GetType().Name);
            foreach (var property in document.RootElement.EnumerateObject())
            {
                property.WriteTo(writer);
            }

        }
        writer.WriteEndObject();
    }
}