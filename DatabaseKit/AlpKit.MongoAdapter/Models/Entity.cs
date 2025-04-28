using AlpKit.Database.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace AlpKit.MongoAdapter.Models;

public class Entity : IEntity
{
    [BsonElement("_id")]
    [BsonId]
    [JsonConverter(typeof(ObjectIdConverter))]
    public Guid Id { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public DateTime? DeletedTime { get; set; }
}

public class ObjectIdConverter : JsonConverter
{

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        return new ObjectId(token.ToObject<string>());
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ObjectId);
    }
}