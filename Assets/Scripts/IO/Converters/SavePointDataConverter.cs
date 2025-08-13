using System;
using Newtonsoft.Json;
using ToB.IO.SubModules;
using ToB.IO.SubModules.SavePoint;

namespace ToB.IO.Converters
{
  public class SavePointDataConverter : JsonConverter<SavePointData>
  {
    public override void WriteJson(JsonWriter writer, SavePointData value, JsonSerializer serializer)
    {
      writer.WriteValue($"{value.stageIndex},{value.roomIndex},{value.pointIndex}");
    }

    public override SavePointData ReadJson(JsonReader reader, Type objectType, SavePointData existingValue, bool hasExistingValue,
      JsonSerializer serializer)
    {
      var str = reader.Value?.ToString();
      if(str == null) return hasExistingValue ? existingValue : SavePointData.Default;
      
      var values = str.Split(',');
      
      return new  SavePointData(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
    }
  }
}