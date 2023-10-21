using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ilmhub.Judge.Messaging.Shared.Converters;

public class AbstractConverter<TAbstract, TConcrete> : JsonConverter<TAbstract>
    where TConcrete : class, TAbstract
{
    public override TAbstract Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<TConcrete>(ref reader, options);

    public override void Write(Utf8JsonWriter writer, TAbstract value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, typeof(TAbstract), options);
}