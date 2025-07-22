using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ToB.IO.Converters
{
  public class Vector3Converter : JsonConverter<Vector3>
  {
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
      writer.WriteValue($"{value.x},{value.y},{value.z}");
    }

    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue,
      JsonSerializer serializer)
    {
      var str = reader.Value?.ToString();
      if(str == null) return hasExistingValue ? existingValue : Vector3.zero;
      
      var values = str.Split(',');

      return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
    }
  }
}