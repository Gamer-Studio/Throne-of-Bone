using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace ToB.Core
{
  /// <summary>
  /// 싱글톤보다 정적 클래스가 접근하기 편할 것 같아 정적 클래스로 구현했습니다.
  /// 각 값은 SoundManager 초기화시 자동으로 불러오며, 값을 설정시 해당 값을 자동으로 저장합니다.
  /// </summary>
  public static class AudioManager
  {
    public const string MasterLabel = "Master";
    public const string BackgroundLabel = "Background";
    public const string UILabel = "UI";
    public const string ObjectLabel = "Object"; 
    public const string Label = "Audio";

    private static AudioListener mainListener = null;
    
    public static bool Loaded { get; private set; } = false;
    public static readonly Dictionary<string, AudioClip> Clips = new();

    private static Camera mainCamera;
    private static AudioSource effectSource = null, backgroundSource = null;
    private static AudioMixerGroup effectGroup, backgroundGroup, objectGroup;
    
    static AudioManager()
    {
      InitScene();

      SceneManager.activeSceneChanged += (_, _) =>
      {
        InitScene();
      };
    }

    private static void InitScene()
    {
      mainCamera = Camera.main;
      effectSource = null;
      backgroundSource = null;
        
      EffectSource?.Stop();
      BackgroundSource?.Stop();

      mainListener = null;
    }

    public static AudioListener Main
    {
      get
      {
        if (mainListener) return mainListener;

        var objs = GameObject.FindGameObjectsWithTag("MainListener");
        var find = false;
        
        foreach (var obj in objs)
        {
          if (obj.TryGetComponent(out mainListener))
          {
            if (!find)
              find = true;
            else
              Debug.LogWarning("Multiple MainListener is found.");
          }
        }

        if(mainListener) return mainListener;
        else throw new InvalidOperationException("MainListener is not found.");
      }
      set => mainListener = value;
    }

    #region Volumes

    private static AudioMixer mixer;
    public static AudioMixer Mixer
    {
      get => mixer;
      set
      {
        mixer = value;
        if (mixer)
        {
          MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 80);
          BackgroundVolume = PlayerPrefs.GetFloat("BackgroundVolume", 80);
          EffectVolume = PlayerPrefs.GetFloat("EffectVolume", 80);
          ObjectVolume = PlayerPrefs.GetFloat("ObjectVolume", 80);
        
          effectGroup = Mixer.FindMatchingGroups(UILabel)[0];
          backgroundGroup = Mixer.FindMatchingGroups(BackgroundLabel)[0];
          objectGroup = Mixer.FindMatchingGroups(ObjectLabel)[0];
        }
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

        if(EffectSource) EffectSource.volume = (input / 100) * (BackgroundVolume / 100);
        if(BackgroundSource) BackgroundSource.volume = (input / 100) * (EffectVolume / 100);
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

        if(BackgroundSource) BackgroundSource.volume = (input / 100) * (MasterVolume / 100);
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
        
        if(EffectSource) EffectSource.volume = (input / 100) * (MasterVolume / 100);
        PlayerPrefs.SetFloat("EffectVolume", input);
      }
    }
    
    /// <summary>
    /// 오브젝트 볼륨입니다.
    /// 값은 0~100 사이로 설정할 수 있고, 미만 혹은 초과시 자동으로 포맷팅됩니다.
    /// </summary>
    public static float ObjectVolume
    {
      get => Mixer && Mixer.GetFloat(ObjectLabel, out var value) ? value + 80 : PlayerPrefs.GetFloat("ObjectVolume", 80);
      set
      {
        var input = Math.Max(0, Math.Min(100, value));

        if(Mixer) Mixer.SetFloat(ObjectLabel, input - 80);
        PlayerPrefs.SetFloat("ObjectVolume", input);
      }
    }

    public static AudioSource EffectSource
    {
      get
      {
        if (!Main) return null;
        
        if (effectSource && effectSource.gameObject == Main.gameObject)
          return effectSource;
        
        effectSource = Main.GetComponents<AudioSource>().FirstOrDefault(s => s.outputAudioMixerGroup == effectGroup);

        if (effectSource) return effectSource;
        
        effectSource = Main.gameObject.AddComponent<AudioSource>();
        effectSource.outputAudioMixerGroup = effectGroup;
        effectSource.volume = (EffectVolume / 100) * (MasterVolume / 100);

        return effectSource;
      }
    }
    
    public static AudioSource BackgroundSource
    {
      get
      {
        if (!Main) return null;
        
        if (backgroundSource && backgroundSource.gameObject == Main.gameObject)
          return backgroundSource;
        
        backgroundSource = Main.GetComponents<AudioSource>().FirstOrDefault(s => s.outputAudioMixerGroup == backgroundGroup);

        if (backgroundSource) return backgroundSource;
        
        backgroundSource = Main.gameObject.AddComponent<AudioSource>();
        backgroundSource.loop = true;
        backgroundSource.outputAudioMixerGroup = backgroundGroup;
        backgroundSource.volume = (BackgroundVolume / 100) * (MasterVolume / 100);

        return backgroundSource;
      }
    }

    #endregion

    #region Controller
    
    public static void Play(AudioClip clip, GameObject obj)
    {
      if(!Mixer) return;

      if (obj.TryGetComponent(out AudioSource source)) {}
      else source = obj.AddComponent<AudioSource>();
      
      source.clip = clip;
      source.outputAudioMixerGroup = objectGroup;
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
      if(!Mixer) return;
      
      var source = type switch
      {
        AudioType.Background => BackgroundSource,
        AudioType.Effect => EffectSource,
        _ => EffectSource
      };
      
      source.clip = clip;
      source.outputAudioMixerGroup = type == AudioType.Background ? backgroundGroup : effectGroup;
      source.Play();
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
      if(!Mixer) return;
      
      var source = type switch
      {
        AudioType.Background => BackgroundSource,
        AudioType.Effect => EffectSource,
        _ => EffectSource
      };
      
      source.Stop();
    }
    
    #endregion
    
    public static (AsyncOperationHandle mixerHandle, AsyncOperationHandle clipHandle) Load()
    {
      if(Loaded) throw new InvalidOperationException("SoundManager is already loaded.");
      Loaded = true;
      
      // Mixer = Addressables.LoadAssetAsync<AudioMixer>(new AssetLabelReference{labelString = Label}).WaitForCompletion();
      var mixerHandle = Addressables.LoadAssetAsync<AudioMixer>(new AssetLabelReference{labelString = Label});
      mixerHandle.Completed += handle => Mixer = handle.Result;

      var clipHandle = Addressables.LoadAssetsAsync<AudioClip>(new AssetLabelReference { labelString = Label }, clip =>
      {
        Clips[clip.name] = clip;
        
        #if UNITY_EDITOR
        GameManager.Instance.loadedAudios[clip.name] = clip;
        #endif
      });
      
      return (mixerHandle, clipHandle);
    }
  }

  public enum AudioType
  {
    Background,
    Effect
  }
}
