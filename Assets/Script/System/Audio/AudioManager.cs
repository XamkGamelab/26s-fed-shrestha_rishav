using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SfxType
{
 BuildingContructionStart,
 BuildingConstructionEnd,
 PlacementSuccess,
 PlacementFail,
}

public enum MusicType
{
    Day,
    Night
}
public enum UIType
{
    Button
}
public enum AmbienceType
{
    BirdChirp
}

public enum MusicTransition
{
    Crossfade,
    Instant
}

[System.Serializable]
public class SoundID<T>
{
    public T Type;
    public AudioClip clip;
}

public static class AudioParameters
{
    public const string MasterVolume = "MasterVolume";
    public const string MusicVolume = "MusicVolume";
    public const string SFXVolume = "FXVolume";
    public const string UIVolume = "UIVolume";
    public const string AmbienceVolume = "AmbienceVolume";
}


public class AudioManager : MonoSingleton<AudioManager>
{
    
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource ambienceSource;
    
    [SerializeField] private AudioMixer MasterMixer;
    
    private AudioSource activeMusicSource;
    private AudioSource inactiveMusicSource;
    
    private float activeMusicStartVolume;
    private float inactiveMusicStartVolume;
    
    
    [SerializeField]private SoundID<SfxType>[] soundIDs;
    [SerializeField]private SoundID<MusicType>[] musicIDs;
    [SerializeField]private SoundID<UIType>[] uiIDs;
    [SerializeField]private SoundID<AmbienceType>[] ambienceIDs;
    
    private Dictionary<SfxType, AudioClip> SoundByType = new();
    private Dictionary<MusicType, AudioClip> MusicByType = new();
    private Dictionary<UIType, AudioClip> UIByType = new();
    private Dictionary<AmbienceType, AudioClip> AmbienceByType = new();
    
    private int ambienceSourceIndex;
    
    private bool isFadingMusic;
    private float musicFadeProgress;
    private float musicFadeDuration = 2f;
    
    
    private void OnEnable()
    {
        GameEventSystem.PlaySound += HandlePlaySound;
        GameEventSystem.PlayMusic += HandlePlayMusic;
        GameEventSystem.StopMusic += HandleStopMusic;
        GameEventSystem.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameEventSystem.PlaySound -= HandlePlaySound;
        GameEventSystem.PlayMusic -= HandlePlayMusic;
        GameEventSystem.StopMusic -= HandleStopMusic;
        GameEventSystem.OnGameStateChanged += HandleGameStateChanged;
    }
    
    
    private void HandlePlaySound(Enum type)
    {
        switch (type)
        {
            case SfxType sfx:
                PlaySound(sfx);
                break;

            case AmbienceType ambience:
                PlayAmbience(ambience);
                break;

            case UIType ui:
                PlayUI(ui);
                break;
        }
    }
    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Paused)
        {
            sfxSource.Pause();
            uiSource.Pause();
            ambienceSource.Pause();
        }
        else if (state == GameState.Playing)
        {
            sfxSource.UnPause();
            uiSource.UnPause();
            ambienceSource.UnPause();
        }
        else if (state == GameState.MainMenu)
        {
            sfxSource.Stop();
            uiSource.Stop();
            ambienceSource.Stop();
        }
    }
    private void HandlePlayMusic(MusicType music, MusicTransition musicTransition=MusicTransition.Crossfade)
    {
        PlayMusic(music, musicTransition);
    }
    private void HandleStopMusic()
    {
        isFadingMusic = false;

        activeMusicSource.Stop();
        inactiveMusicSource.Stop();

        activeMusicSource.clip = null;
        inactiveMusicSource.clip = null;

        activeMusicSource.volume = 1f;
        inactiveMusicSource.volume = 0f;
    }
    
    public override void Awake()
    {
        base.Awake();

        activeMusicSource = musicSourceA;
        inactiveMusicSource = musicSourceB;

        musicSourceA.loop = true;
        musicSourceB.loop = true;

        activeMusicSource.volume = 1f;
        inactiveMusicSource.volume = 0f;

        InitializeSoundDictionary(soundIDs, SoundByType, "Sound");
        InitializeSoundDictionary(musicIDs, MusicByType, "Music");
        InitializeSoundDictionary(uiIDs, UIByType, "UI");
        InitializeSoundDictionary(ambienceIDs, AmbienceByType, "Ambience");
    }
    private void InitializeSoundDictionary<T>(
        SoundID<T>[] soundIDs,
        Dictionary<T, AudioClip> dictionary,
        string categoryName) where T : Enum
    {
        foreach (var soundID in soundIDs)
        {
            if (!dictionary.TryAdd(soundID.Type, soundID.clip))
            {
                Debug.LogWarning(
                    $"Duplicate {categoryName} type: {soundID.Type}");
            }
        }
    }
    
    private void Update()
    {
        if (!isFadingMusic)
            return;

        if (musicFadeDuration <= 0f)
        {
            FinishMusicCrossfade();
            return;
        }

        musicFadeProgress += Time.deltaTime / musicFadeDuration;

        activeMusicSource.volume = Mathf.Lerp(
            activeMusicStartVolume,
            0f,
            musicFadeProgress
        );

        inactiveMusicSource.volume = Mathf.Lerp(
            inactiveMusicStartVolume,
            1f,
            musicFadeProgress
        );

        if (musicFadeProgress >= 1f)
            FinishMusicCrossfade();
    }

    public void PlaySound(SfxType soundType)
    {
        if (!SoundByType.TryGetValue(soundType, out AudioClip clip))
        {
            Debug.Log($"[nameof(AudioManager)] Sound Type not found: {soundType} [ {soundType.ToString()}]");
            return;
        }
        sfxSource.PlayOneShot(clip);
    }
    
    public void PlayUI(UIType uiType)
    {
        if (!UIByType.TryGetValue(uiType, out AudioClip clip))
        {
            Debug.Log($"[nameof(AudioManager)] UI Type not found: {uiType} [ {uiType.ToString()}]");
            return;
        }
        uiSource.PlayOneShot(clip);
    }
    public void PlayAmbience(AmbienceType ambienceType)
    {
        if (!AmbienceByType.TryGetValue(ambienceType, out AudioClip clip))
        {
            Debug.Log($"[nameof(AudioManager)] Ambience Type not found: {ambienceType} [ {ambienceType.ToString()}]");
            return;
        }
        ambienceSource.PlayOneShot(clip);
    }
    
    public void PlayMusic(
        MusicType type,
        MusicTransition transition)
    {
        if (!MusicByType.TryGetValue(type, out AudioClip clip))
            return;

        if (transition == MusicTransition.Instant)
        {
            isFadingMusic = false;

            activeMusicSource.Stop();
            inactiveMusicSource.Stop();

            activeMusicSource.clip = clip;
            activeMusicSource.volume = 1f;
            activeMusicSource.Play();

            inactiveMusicSource.volume = 0f;
            return;
        }

        if (activeMusicSource.clip == clip && activeMusicSource.isPlaying)
            return;

        StartMusicCrossfade(clip);
    }
    
    private void StartMusicCrossfade(AudioClip newClip)
    {
        inactiveMusicSource.Stop();
        inactiveMusicSource.clip = newClip;

        activeMusicStartVolume = activeMusicSource.volume;
        inactiveMusicStartVolume = inactiveMusicSource.volume;

        inactiveMusicSource.Play();

        musicFadeProgress = 0f;
        isFadingMusic = true;
    }
    private void SetMixerVolume(string parameter, float volume)
    {
        if (MasterMixer == null)
            return;

        volume = Mathf.Clamp(volume, 0.0001f, 1f);

        float dbVolume = Mathf.Log10(volume) * 20f;
        MasterMixer.SetFloat(parameter, dbVolume);
    }
    
    public void SetMasterVolume(float volume)
        => SetMixerVolume(AudioParameters.MasterVolume, volume);

    public void SetMusicVolume(float volume)
        => SetMixerVolume(AudioParameters.MusicVolume, volume);

    public void SetSFXVolume(float volume)
        => SetMixerVolume(AudioParameters.SFXVolume, volume);

    public void SetAmbienceVolume(float volume)
        => SetMixerVolume(AudioParameters.AmbienceVolume, volume);

    private void FinishMusicCrossfade()
    {
        activeMusicSource.Stop();
        activeMusicSource.clip = null;
        activeMusicSource.volume = 0f;

        inactiveMusicSource.volume = 1f;

        (activeMusicSource, inactiveMusicSource) =
            (inactiveMusicSource, activeMusicSource);

        isFadingMusic = false;
    }

}
