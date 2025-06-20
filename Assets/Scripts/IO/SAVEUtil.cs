using System.IO;
using System.Threading.Tasks;

namespace ToB.IO
{
  public static class SAVEUtil
  {
    public static async Task<SAVE> LoadSave(this FileInfo info, bool general = true)
    {
      var data = await SAVE.Load(info.Name);
      
      if(general) SAVE.Current = data;
      
      return data;
    }

    public static SAVE LoadSaveSync(this FileInfo info, bool general = true)
    {
      var task = LoadSave(info, general);
      task.Wait();
      return task.Result;
    }
  }
}