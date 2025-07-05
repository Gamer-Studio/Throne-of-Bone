using System.Collections;
using NUnit.Framework;
using ToB.IO;
using UnityEngine.TestTools;

namespace ToB.Editor
{
  public class Test
  {
    [Test]
    public void SAVETest()
    {
      var test = new SAVE("Test");
      
      test.Node("Table1", true);
      test.Node("Table2", true);
      test.Node("Table3", true);
      test.Node("Table1", true).Node("Child1", true);
      test.Node("Table1", true).Node("Child2", true);
      test.Node("Table1", true).Node("Child3", true);
      
      test.Save();
    }
  }
}