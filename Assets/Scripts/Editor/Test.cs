using System.Threading.Tasks;
using NUnit.Framework;
using ToB.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ToB.Editor
{
  public class Test
  {
    [Test]
    public async Task SAVETest()
    {
      var save = (await SAVE.GetAllSaves())[0];

      // save.LoadAll().Wait();
      
      save.name = "Test";
      save.Save();
    }

    [Test]
    public async Task SAVELoadTest()
    {
      var save = await SAVE.Load("Test");
    }

    public Button button;
    [Test]
    public void LoadAllSAVETest()
    {
      button.Select();
      // button.navigation.selectOnDown;
      
      var saves = SAVE.GetAllSaves().Result;

      foreach (var save in saves)
      {
        Debug.Log(save.MetaData);
      }
    }
  }
}