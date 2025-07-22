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
    public const string MasterLabel = "Master";
    public const string BackgroundLabel = "Background";
    public const string UILabel = "UI";
    public const string ObjectLabel = "Object"; 
    public const string Label = "Audio";

    [SerializeField] private AudioListener mainListener = null;
    
    public static bool Loaded { get; private set; } = false;
    public static readonly Dictionary<string, AudioClip> Clips = new();

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource backgroundSource = null;
    [SerializeField] private List<AudioSource> effectSources = new(); 
    
    [SerializeField] private AudioMixerGroup effectGroup, backgroundGroup, objectGroup;
    
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
      if(!Loaded) return;
      
      if(Clips.TryGetValue(clipName, out var clip)) Play(clip, obj);
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
      if(!Loaded) return;
      
      if(Clips.TryGetValue(clipName, out var clip)) Play(clip, type);
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
    
    public static AsyncOperationHandle Load()
    {
      if(Loaded) throw new InvalidOperationException("SoundManager is already loaded.");
      Loaded = true;
      
      var clipHandle = Addressables.LoadAssetsAsync<AudioClip>(new AssetLabelReference { labelString = Label }, clip =>
      {
        Clips[clip.name] = clip;
        
        #if UNITY_EDITOR
        GameManager.Instance.loadedAudios[clip.name] = clip;
        #endif
      });
      
      return clipHandle;
    }
  }

  public enum AudioType
  {
    Background,
    Effect
  }
}
