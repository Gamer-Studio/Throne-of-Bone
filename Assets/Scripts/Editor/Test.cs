using System.Collections;
using System.Threading.Tasks;
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
      
      var table1 = test.Node("Table1", true);
      
      table1["Child1"] = 1;
      table1["da"] = "1";
      table1["Chidv"] = true;
      
      test.Node("Table2", true);
      test.Node("Table3", true);
      test.Node("Table1", true).Node("Child1", true);
      test.Node("Table1", true).Node("Child2", true);
      test.Node("Table1", true).Node("Child3", true);
      
      test.Save();
    }

    [Test]
    public async Task SAVELoadTest()
    {
      var save = await SAVE.Load("Test");
    }
  }
}