using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR

namespace ToB.Utils
{
  public class DebugSymbol
  {
    #region Definition

    public static readonly HashSet<DebugSymbol> Symbols = new();
    
    public static readonly DebugSymbol 
      Editor = new ("EDITOR"),
      UI = new ("UI"),
      Save = new ("SAVE");
    
    #endregion

    public readonly string name;
    public readonly bool dontDestroy;
    public bool isActive = false;
    public event Action onRelease;
    
    private DebugSymbol(string name, bool dontDestroy = true)
    {
      this.name = name;
      this.dontDestroy = dontDestroy;
      
      Symbols.Add(this);
    }

    public static DebugSymbol Get(string name, bool force = false)
    {
      var result = Symbols.FirstOrDefault(x => x.name == name);

      if (result != null) return result;
      
      if (!force) return null;
      
      var newSymbol = new DebugSymbol(name);
      Symbols.Add(newSymbol);
      return newSymbol;
    }

    public static DebugSymbol Create(string name, bool dontDestroy = false)
    {
      var result = Symbols.FirstOrDefault(x => x.name == name);

      if (result != null) return result;
      
      var newSymbol = new DebugSymbol(name, dontDestroy);
      Symbols.Add(newSymbol);
      return newSymbol;
    }

    public static bool IsActive(params string[] symbols)
    {
      var active = true;
      
      foreach (var symbol in symbols)
      {
        var target = Get(symbol, false);
        if (target == null)
        {
          active = false;
          break;
        }
        
        active &= target.isActive;
        
        if (!active) break;
      }
      
      return active;
    }

    public void Release()
    {
      Symbols.Remove(this);
      onRelease?.Invoke();
    }
    
    #region Operator

    public static implicit operator DebugSymbol(string symbol) => Get(symbol, true);
    public static implicit operator bool(DebugSymbol symbol) => symbol.isActive;

    #endregion
  }
}

#endif