namespace ToB.IO
{
  public class GenericSAVEModule <T> : SAVEModule where T : IJsonSerializable
  {
    public GenericSAVEModule(string name) : base(name)
    {
      
    }
    
    public void Bind(ref T content)
    {
      throw new System.NotImplementedException();
    }
  }
}