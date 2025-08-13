using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ToB.Memories
{
    [CreateAssetMenu(fileName = "MemoriesData", menuName = "ToB/Memories/MemoriesData")]
    public class MemoriesDataSO : ScriptableObject
    {
       public List<Memories> memoriesDataBase = new List<Memories>();
       
       public Memories GetMemoriesById(int id)
       {
           return memoriesDataBase.Find(x => x.id == id);
       }
       
       public Memories GetMemoriesByName(string name)
       {
           return memoriesDataBase.Find(x => x.name == name);
       }
    }
    
    [Serializable]
    public class Memories
    {
        public int id;
        public string name;
        public string description;
        public string relatedIconFileName;
        public Sprite relatedIcon;
        
        
        
    }
}