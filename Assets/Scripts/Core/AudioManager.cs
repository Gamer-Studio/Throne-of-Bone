using System;
using System.Collections.Generic;
using System.Linq;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace ToB.Core
{
  public class AudioManager : PrefabSingleton<AudioManager>
  {
    public const string ObjectLabel = "Object"; 
    public const string Label = "Audio";

    private static readonly Dictionary<string, AudioClip> Clips = new();
    private static readonly Dictionary<string, AssetReference> References = new();
    
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource backgroundSource = null;
    [SerializeField] private List<AudioSource> effectSources = new(); 
    
    [SerializeField] private AudioMixerGroup effectGroup, backgroundGroup, objectGroup;
    
#if UNITY_EDITOR
    [SerializeField] private SerializableDictionary<string, AudioClip> loadedClips = new();
#endif
    
    #region Unity Event

    protected override void Awake()
    {
      base.Awake();
      
      MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 80);
      BackgroundVolume = PlayerPrefs.GetFloat("BackgroundVolume", 80);
      EffectVolume = PlayerPrefs.GetFloat("EffectVolume", 80);
      ObjectVolume = PlayerPrefs.GetFloat("ObjectVolume", 80);
    }
    
    #endregion
    
    #region Volumes

    private void SetEffectVolume(float value)
    {
      if(effectSources == null) return;
      
      foreach (var source in effectSources)
      {
        source.volume = value;
      }
    }

    /// <summary>
    /// 각 볼륨을 총괄하는 마스터 볼륨입니다.
    /// 해당 볼륨을 변경시 다른 모든 볼륨이 영향받습니다.
    /// 값은 0~100 사이로 설정할 수 있고, 미만 혹은 초과시 자동으로 포맷팅됩니다.
    /// </summary>
    public static float MasterVolume
    {
      get => PlayerPrefs.GetFloat("MasterVolume", 80);
      set
      {
        var input = Math.Max(0, Math.Min(100, value));

        if (HasInstance)
        {
          Instance.SetEffectVolume((input / 100) * (BackgroundVolume / 100));
          Instance.backgroundSource.volume = (input / 100) * (EffectVolume / 100);
        }
        PlayerPrefs.SetFloat("MasterVolume", input);
      }
    }

    /// <summary>
    /// 배경 볼륨입니다.
    /// 값은 0~100 사이로 설정할 수 있고, 미만 혹은 초과시 자동으로 포맷팅됩니다.
    /// </summary>
    public static float BackgroundVolume
    {
      get => PlayerPrefs.GetFloat("BackgroundVolume", 80);
      set
      {
        var input = Math.Max(0, Math.Min(100, value));

        if (HasInstance)
          Instance.backgroundSource.volume = (input / 100) * (MasterVolume / 100);

        PlayerPrefs.SetFloat("BackgroundVolume", input);
      }
    }

    /// <summary>
    /// 효과음 볼륨입니다.
    /// 값은 0~100 사이로 설정할 수 있고, 미만 혹은 초과시 자동으로 포맷팅됩니다.
    /// </summary>
    public static float EffectVolume
    {
      get => PlayerPrefs.GetFloat("EffectVolume", 80);
      set
      {
        var input = Math.Max(0, Math.Min(100, value));
        
        if (HasInstance) 
          Instance.SetEffectVolume((input / 100) * (MasterVolume / 100));
        
        PlayerPrefs.SetFloat("EffectVolume", input);
      }
    }
    
    /// <summary>
    /// 오브젝트 볼륨입니다.
    /// 값은 0~100 사이로 설정할 수 있고, 미만 혹은 초과시 자동으로 포맷팅됩니다.
    /// </summary>
    public static float ObjectVolume
    {
      get => HasInstance && Instance.mixer.GetFloat(ObjectLabel, out var value) ? value + 80 : PlayerPrefs.GetFloat("ObjectVolume", 80);
      set
      {
        var input = Math.Max(0, Math.Min(100, value));

        if (HasInstance) 
          Instance.mixer.SetFloat(ObjectLabel, input - 80);
        
        PlayerPrefs.SetFloat("ObjectVolume", input);
      }
    }

    #endregion

    public static AudioClip GetClip(string name)
    {
      if (Clips.TryGetValue(name, out var clip)) return clip;

      var reference = new AssetReference($"{Label}/{name}");

      try
      {
        clip = reference.LoadAssetAsync<AudioClip>().WaitForCompletion();
      }
      catch (Exception e)
      {
        Debug.LogError(e);
        return null;
      }

      Clips[name] = clip;
#if UNITY_EDITOR
      if(HasInstance)
        Instance.loadedClips[name] = clip;
#endif
      
      return clip;
    }

    public static bool TryGetClip(string name, out AudioClip clip)
    {
      clip = GetClip(name);
      
      return clip != null;
    }

    public static bool ReleaseClip(string name)
    {
      if (References.TryGetValue(name, out var reference))
      {
        Clips[name].UnloadAudioData();
        Clips.Remove(name);
        reference.ReleaseAsset();
        
#if UNITY_EDITOR
        if (HasInstance)
          Instance.loadedClips.Remove(name);
#endif
        
        return true;
      }
      else return false;
    }
    
    #region Controller
    
    public static void Play(AudioClip clip, GameObject obj)
    {
      if(!HasInstance) return;

      if (obj.TryGetComponent(out AudioSource source)) {}
      else source = obj.AddComponent<AudioSource>();
      
      source.clip = clip;
      source.outputAudioMixerGroup = Instance.objectGroup;
      source.Play();
    }

    public static void Play(string clipName, GameObject obj)
    {
      if(TryGetClip(clipName, out var clip)) Play(clip, obj);
      #if UNITY_EDITOR
      else Debug.LogWarning($"AudioClip {clipName} is not found.");
      #endif
    }

    public static void Play(AudioClip clip, AudioType type = AudioType.Effect)
    {
      if(!HasInstance) return;

      if (type == AudioType.Background)
      {
        Instance.backgroundSource.clip = clip;
        Instance.backgroundSource.Play();
      }
      else
      {
        var availableSource = (from source in Instance.effectSources where source.isPlaying == false select source);

        var audioSources = availableSource as AudioSource[] ?? availableSource.ToArray();
        if (audioSources.Any())
        {
          audioSources.First().clip = clip;
          audioSources.First().Play();
        }
        else
        {
          // 없을 경우 생성하여 붙이기
          var obj = new GameObject("EffectSource");
          obj.transform.SetParent(Instance.transform);
          obj.transform.localPosition = Vector3.zero;
          var source = obj.AddComponent<AudioSource>();

          source.clip = clip;
          source.Play();
          
          Instance.effectSources.Add(source);
        }
      }
    }
    
    public static void Play(string clipName, AudioType type = AudioType.Effect)
    {
      if(TryGetClip(clipName, out var clip)) Play(clip, type);
      #if UNITY_EDITOR
      else Debug.LogWarning($"AudioClip {clipName} is not found.");
      #endif
    }

    public static void Stop(AudioType type = AudioType.Background)
    {
      if(!HasInstance) return;

      if (type == AudioType.Background)
        Instance.backgroundSource.Stop();
      else
      {
        foreach (var source in Instance.effectSources)
        {
          source.Stop();
        }
      }
      
    }
    
    #endregion
  }

  public enum AudioType
  {
    Background,
    Effect
  }
}
