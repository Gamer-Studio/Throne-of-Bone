using Newtonsoft.Json.Linq;

namespace ToB.IO
{
  public interface IJsonSerializable
  {
    void LoadJson(JObject json);
    JObject ToJson();
  }
}