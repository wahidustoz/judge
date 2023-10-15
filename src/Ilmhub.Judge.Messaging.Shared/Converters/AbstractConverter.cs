using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ilmhub.Judge.Messaging.Shared;

public class AbstractConverter<IAbstract, TConcrete> : JsonConverter<IAbstract>
    where TConcrete : class, IAbstract
{
    public override IAbstract Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<TConcrete>(ref reader, options);

    public override void Write(Utf8JsonWriter writer, IAbstract value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, typeof(IAbstract), options);
}
