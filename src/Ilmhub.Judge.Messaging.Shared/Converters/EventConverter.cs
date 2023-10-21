using System.Text.Json;
using System.Text.Json.Serialization;
using Ilmhub.Judge.Messaging.Shared.Events;
using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging.Shared.Converters;

public class EventConverter : JsonConverter<IJudgeEvent>
{
    private const string DESCRIMINATOR = "$event";
    private readonly Type[] types = new Type[]
    {
        typeof(JudgeCompleted),
        typeof(JudgeFailed),
        typeof(RunCompleted),
        typeof(RunFailed)
    };

    public override bool CanConvert(Type type)
        => typeof(IJudgeEvent).IsAssignableFrom(type);

    public override IJudgeEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject)
            throw new JsonException();

        using var jsonDocument = JsonDocument.ParseValue(ref reader);

        if (false == jsonDocument.RootElement.TryGetProperty(DESCRIMINATOR, out var typeProperty))
            throw new JsonException();

        var type = types.FirstOrDefault(x => x.Name == typeProperty.GetString())
            ?? throw new JsonException();

        var jsonObject = jsonDocument.RootElement.GetRawText();
        var result = (IJudgeEvent)JsonSerializer.Deserialize(jsonObject, type);

        return result;
    }

    public override void Write(Utf8JsonWriter writer, IJudgeEvent value, JsonSerializerOptions options)
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