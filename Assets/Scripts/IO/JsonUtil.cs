using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ToB.IO
{
  public static class JsonUtil
  {
    /// <summary>
    /// 비어 있는 더미 JObject입니다. 주로 초기값으로 사용됩니다.
    /// </summary>
    public static readonly JObject Blank = new JObject(); 
    
    /// <summary>
    /// 열거형 값을 정수 기반의 JValue로 변환합니다.
    /// </summary>
    /// <typeparam name="T">변환할 열거형 타입</typeparam>
    /// <param name="enumValue">변환할 열거형 값</param>
    /// <returns>정수 값을 가지는 JValue</returns>
    public static JValue ToToken<T>(this T enumValue) where T : struct, Enum
    {
      var value = Convert.ToInt32(enumValue);
      return new JValue(value);
    }
    
    /// <summary>
    /// JToken에서 열거형 값으로 변환합니다.
    /// </summary>
    /// <typeparam name="T">변환할 열거형 타입</typeparam>
    /// <param name="token">변환 대상 JToken</param>
    /// <param name="defaultValue">값이 정의되지 않았을 경우 반환할 기본값</param>
    /// <returns>변환된 열거형 값 또는 기본값</returns>
    public static T ToEnum<T>(this JToken token, T defaultValue = default) where T : struct, Enum
    {
      var value = token.Value<int>();

      return Enum.IsDefined(typeof(T), value) ? (T) Enum.ToObject(typeof(T), token.Value<int>()) : defaultValue;
    }

    /// <summary>
    /// 다양한 타입의 값을 JObject로 변환합니다.
    /// </summary>
    /// <typeparam name="T">변환할 값의 타입</typeparam>
    /// <param name="value">변환할 값</param>
    /// <returns>변환된 JObject</returns>
    public static JObject ToJObject<T>(this T value) => value switch
    {
      int i => new JObject(i),
      float f => new JObject(f),
      string s => new JObject(s),
      bool b => new JObject(b),
      IJsonSerializable jsonSerializable => jsonSerializable.ToJson(),
      _ => new JObject(value.ToString())
    };
    

    /// <summary>
    /// Dictionary를 JObject로 변환합니다.
    /// </summary>
    /// <typeparam name="T">값의 타입</typeparam>
    /// <param name="dictionary">변환할 Dictionary</param>
    /// <returns>변환된 JObject</returns>
    public static JObject DicToJObject<T>(this IDictionary<string, T> dictionary)
    {
      var json = new JObject();
      
      foreach (var (key, value) in dictionary)
      {
        json[key] = value.ToJObject();
      }

      return json;
    }

    /// <summary>
    /// JObject를 Dictionary로 변환합니다.
    /// </summary>
    /// <typeparam name="T">값의 타입</typeparam>
    /// <param name="json">변환할 JObject</param>
    /// <returns>변환된 Dictionary</returns>
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

    #region JObject
    /// <summary>
    /// 지정된 키에 해당하는 열거형 값을 가져옵니다.
    /// </summary>
    /// <typeparam name="T">열거형 타입</typeparam>
    /// <param name="json">JObject 소스</param>
    /// <param name="key">가져올 키 이름</param>
    /// <param name="defaultValue">값이 없거나 유효하지 않을 때 반환할 기본값</param>
    /// <returns>열거형 값 또는 기본값</returns>
    public static T GetEnum<T>(this JObject json, string key, T defaultValue = default) where T : struct, Enum
    {
      if (json.TryGetValue(key, out var token))
      {
        var value = token.Value<int>();
        return Enum.IsDefined(typeof(T), value) ? (T) Enum.ToObject(typeof(T), value) : defaultValue;
      }
      
      return defaultValue;
    }

    /// <summary>
    /// 지정된 키에 해당하는 값을 가져옵니다.
    /// </summary>
    /// <typeparam name="T">값의 타입</typeparam>
    /// <param name="json">JObject 소스</param>
    /// <param name="key">가져올 키</param>
    /// <param name="defaultValue">기본값</param>
    /// <returns>가져온 값 또는 기본값</returns>
    public static T Get<T>(this JObject json, string key, T defaultValue = default)
      => json.TryGetValue(key, out var token) ? token.Value<T>() : defaultValue;

    /// <summary>
    /// 지정된 키에 해당하는 Vector3 값을 가져옵니다.
    /// </summary>
    /// <param name="json">JObject 소스</param>
    /// <param name="key">가져올 키</param>
    /// <param name="defaultValue">기본 Vector3 값</param>
    /// <returns>Vector3 값 또는 기본값</returns>
    public static Vector3 Get(this JObject json, string key, Vector3 defaultValue)
    {
      var data = json.Get<JObject>(key);

      return data != null ? new Vector3(data.Get("x", 0), data.Get("y", 0), data.Get("z", 0)) : defaultValue;
    }
    
    #endregion JObject

    #region JToken
    /// <summary>
    /// JToken에서 값을 가져옵니다.
    /// </summary>
    /// <typeparam name="T">값의 타입</typeparam>
    /// <param name="token">JToken 객체</param>
    /// <param name="defaultValue">기본값</param>
    /// <returns>가져온 값 또는 기본값</returns>
    public static T Get<T>(this JToken token, T defaultValue = default)
      => token.Value<T>() ?? defaultValue;
    
    /// <summary>
    /// JToken에서 열거형 값을 가져옵니다.
    /// </summary>
    /// <typeparam name="T">열거형 타입</typeparam>
    /// <param name="token">JToken 객체</param>
    /// <param name="defaultValue">값이 없거나 잘못되었을 때 반환할 기본값</param>
    /// <returns>열거형 값 또는 기본값</returns>
    public static T GetEnum<T>(this JToken token, T defaultValue = default) where T : struct, Enum
    {
      if (token == null || token.Type == JTokenType.Null || token.Type == JTokenType.Undefined)
        return defaultValue;

      var value = token.Value<int>();

      return Enum.IsDefined(typeof(T), value)
        ? (T)Enum.ToObject(typeof(T), value)
        : defaultValue;
    }
    
    #endregion JToken
    
    #endregion Getter

    #region Setter

    /// <summary>
    /// 열거형 값을 정수로 변환하여 JObject에 설정합니다.
    /// </summary>
    /// <typeparam name="T">열거형 타입</typeparam>
    /// <param name="json">설정할 JObject</param>
    /// <param name="key">설정할 키</param>
    /// <param name="value">설정할 열거형 값</param>
    /// <returns>설정된 정수 값</returns>
    public static int Set<T>(this JObject json, string key, T value) where T : struct, Enum
    {
      var token = Convert.ToInt32(value);
      json[key] = token;
      
      return token;
    }

    /// <summary>
    /// Vector3 값을 JObject로 설정합니다.
    /// </summary>
    /// <param name="json">설정할 JObject</param>
    /// <param name="key">설정할 키</param>
    /// <param name="vector">설정할 Vector3 값</param>
    /// <returns>변경된 JObject</returns>
    public static JObject Set(this JObject json, string key, Vector3 vector)
    {
      var obj = new JObject
      {
        ["x"] = vector.x,
        ["y"] = vector.y,
        ["z"] = vector.z
      };
      
      json[key] = obj;
      return json;
    }

    #endregion
  }
}