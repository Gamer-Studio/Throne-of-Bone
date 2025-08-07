using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ToB.IO.Converters
{
  public class IntEnumDictionaryConverter <V> : JsonConverter<Dictionary<int, V>> where V : Enum
  {
    public override void WriteJson(JsonWriter writer, Dictionary<int, V> dictionary, JsonSerializer serializer)
    {
      var data = new JObject();

      foreach (var (key, value) in dictionary)
      {
        // data[key.ToString()] = Convert.ToInt32(value);
        data[key.ToString()] = value.ToString();
      }
      
      data.WriteTo(writer);
    }

    public override Dictionary<int, V> ReadJson(JsonReader reader, Type objectType, Dictionary<int, V> existingValue, bool hasExistingValue,
      JsonSerializer serializer)
    {
      var result = hasExistingValue && existingValue != null
        ? new Dictionary<int, V>(existingValue) // 기존 값을 복사해서 사용
        : new Dictionary<int, V>();
      JObject json;
      
      try
      {
        // JSON을 JObject로 로드
        json = JObject.Load(reader);
      }
      catch (JsonReaderException)
      {
        return result;
      }
      
      foreach (var (key, value) in json)
      {
        // 문자열 키 → int 변환
        if (!int.TryParse(key, out var id)) continue;
        if (value is not { Type: JTokenType.Integer }) continue;
        
        // 문자열 값 → Enum 변환
        
        result[id] = Enum.IsDefined(typeof(V), value) ? (V) Enum.ToObject(typeof(V), value) : default;
      }

      return result;
    }
  }
}