namespace ToB.IO.SubModules
{
  public class PlayerModule : SAVEModule
  {
    protected override string ModuleType => nameof(PlayerModule);

    public PlayerModule(string name) : base(name)
    {
    }
  }
}