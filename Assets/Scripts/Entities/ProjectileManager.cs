using System;
using System.Collections.Generic;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.Pool;

namespace ToB
{
  public class ProjectileManager : Singleton<ProjectileManager>
  {
    private readonly Dictionary<string, ObjectPool<GameObject>> pools = new();
    private readonly Dictionary<string, Transform> releasedContainers = new();

    /// <summary>
    /// key 값의 오브젝트를 풀링합니다.
    /// </summary>
    /// <param name="key">풀링할 게임오브젝트의 키입니다.</param>
    /// <returns>풀링한 게임오브젝트입니다.</returns>
    public GameObject Get(string key)
    {
      if (pools.TryGetValue(key, out var pool))
      {
        return pool.Get();
      }
      
      throw new MissingReferenceException($"{key}는 존재하지 않는 오브젝트 풀입니다.");
    }
    
    /// <summary>
    /// 풀링한 오브젝트를 해제합니다.
    /// 풀링되 오브젝트가 아닐경우 파괴합니다.
    /// </summary>
    /// <param name="obj">해제할 오브젝트입니다.</param>
    public void Release(GameObject obj)
    {
      if(!obj.transform.parent)
      {
        Destroy(obj);
        return;
      }
        
      var containerName = obj.transform.parent.name;
      if (pools.TryGetValue(containerName, out var pool))
        pool.Release(obj);
      else Destroy(obj);
    }

    /// <summary>
    /// 프리팹을 key의 오브젝트 풀로 생성합니다.
    /// </summary>
    /// <param name="key">풀의 키입니다.</param>
    /// <param name="prefab">풀의 원본 프리팹입니다.</param>
    /// <returns>생성된 오브젝트 풀입니다.</returns>
    public ObjectPool<GameObject> Register(string key, GameObject prefab)
    {
      if (pools.ContainsKey(key)) throw new Exception($"해당 {key} 풀은 이미 생성되어있습니다.");

      var container = new GameObject(key);
      container.transform.SetParent(transform);
      releasedContainers.Add(key, container.transform);
      
      var result = new ObjectPool<GameObject>(() =>
      {
        // create
        var obj = Instantiate(prefab, container.transform);

        return obj;
      }, obj =>
      {
        // get
        obj.SetActive(true);
      }, obj =>
      {
        // release
        obj.SetActive(false);
      }, Destroy);

      pools.Add(key, result);
      return result;
    }
    
    /// <param name="key">확인할 풀의 키 입니다.</param>
    /// <returns>해당 키의 오브젝트 풀이 만들어져있는지 여부</returns>
    public bool IsRegistered(string key) => pools.ContainsKey(key);

    private void OnDestroy()
    {
      foreach (var (_, pool) in pools)
      {
        pool.Dispose();
      }
    }
  }
}