using System;
using System.Text.Json;
using System.Text.Json.Serialization;

   
namespace HelperLayer.AIAgent;   

public class DateTimeConverter : JsonConverter<DateTime?>{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options){
        if (reader.TokenType == JsonTokenType.Null)
            return null;
            
        if (reader.TokenType == JsonTokenType.String){
            var value = reader.GetString();
            if (DateTime.TryParse(value, out var date))
                return date;
        }
            
            return null;
        }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options){
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
        else
            writer.WriteNullValue();
    }
}