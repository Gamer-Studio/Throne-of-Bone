using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ToB.IO
{
  public static class JsonUtil
  {
    public static JValue ToToken<T>(this T enumValue) where T : struct, Enum
    {
      var value = Convert.ToInt32(enumValue);
      return new JValue(value);
    }
    
    public static T ToEnum<T>(this JToken token, T defaultValue = default) where T : struct, Enum
    {
      var value = token.Value<int>();

      return Enum.IsDefined(typeof(T), value) ? (T) Enum.ToObject(typeof(T), token.Value<int>()) : defaultValue;
    }

    public static JObject ToJObject<T>(this T value) => value switch
    {
      int i => new JObject(i),
      float f => new JObject(f),
      string s => new JObject(s),
      bool b => new JObject(b),
      IJsonSerializable jsonSerializable => jsonSerializable.ToJson(),
      _ => new JObject(value.ToString())
    };
    

    public static JObject DicToJObject<T>(this IDictionary<string, T> dictionary)
    {
      var json = new JObject();
      
      foreach (var (key, value) in dictionary)
      {
        json[key] = value.ToJObject();
      }

      return json;
    }

    public static Dictionary<string, T> ToDictionary<T>(this JObject json)
    {
      var result = new Dictionary<string, T>();
      
      foreach (var (key, value) in json)
      {
        if (value is not null)
        {
          result[key] = value.ToObject<T>();
        }
      }
      
      return result;
    }
    
    #region Getter 

    public static T GetEnum<T>(this JObject json, string key, T defaultValue = default) where T : struct, Enum
    {
      if (json.TryGetValue(key, out var token))
      {
        var value = token.Value<int>();
        return Enum.IsDefined(typeof(T), value) ? (T) Enum.ToObject(typeof(T), value) : defaultValue;
      }
      
      return defaultValue;
    }

    public static T Get<T>(this JObject json, string key, T defaultValue = default)
      => json.TryGetValue(key, out var token) ? token.Value<T>() : defaultValue;
    
    #endregion

    #region Setter

    public static int Set<T>(this JObject json, string key, T value) where T : struct, Enum
    {
      var token = Convert.ToInt32(value);
      json[key] = token;
      
      return token;
    }

    #endregion
  }
}