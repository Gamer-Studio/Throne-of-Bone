using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ToB.IO.SubModules
{
  public abstract class SubModule : ISAVEModule
  {
    protected string name;
    public SubModule(string name) {}
    public abstract string ModuleType { get; }

    public virtual string Name
    {
      get => name;
      set => name = value;
    }

    public virtual JObject BeforeSave()
    {
      return new JObject();
    }
    public virtual void Read(JObject data) {}

    public virtual void Save(string parentPath)
    {
      
    }
    
    public abstract Task Load(string path, bool chainLoading);
    public abstract SAVEModule Node(string key, bool force = false);
    public abstract T Node<T>(string key, bool force = false) where T : ISAVEModule;
  }
}