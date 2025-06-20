using System;

namespace ToB.Utils
{
  public static class EnumUtil
  {
    /// <summary>
    ///   열거형의 목록을 가져와서 기존 열거형 값이랑 비교 후 다음 열거형을 반환
    /// </summary>
    /// <param name="value">원본 열거형의 값</param>
    /// <param name="reverse">역방향으로 가져올 것인지</param>
    /// <typeparam name="T">값을 받아올 열거형의 종류</typeparam>
    /// <returns></returns>
    public static T Next<T>(this T value, bool reverse = false) where T : struct, Enum
    {
      var enums = (T[])Enum.GetValues(typeof(T));
      var index = Array.IndexOf(enums, value);
      index = reverse ? index - 1 : index + 1;

      return enums[index < 0 ? enums.Length - 1 : index > enums.Length - 1 ? 0 : index];
    }
  }
}