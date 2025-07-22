using ToB.Utils;

namespace ToB.Entities
{
  public class Entity : PooledObject
  {
    public EntityType type;
    
  }

  public enum EntityType
  {
    Common,
    Elite,
    Boss,
    Npc
  }
}