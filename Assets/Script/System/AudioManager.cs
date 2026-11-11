using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SfxType
{
 BuildingContructionStart,
 BuildingConstructionEnd,
 AmbienceGoldCoinsClinking
}

public enum MusicType
{
    Day,
    Night
}



public class AudioManager : MonoBehaviour
{
    
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource ambienceSource;
    
    [SerializeField] private AudioMixer MasterMixer;
    
    public Dictionary<SfxType, AudioClip> SoundByType = new();
    public Dictionary<MusicType, AudioClip> MusicByType = new();


    public void PlaySound(SfxType soundType)
    {
        if (!SoundByType.TryGetValue(soundType, out AudioClip clip))
        {
            Debug.Log($"[nameof(AudioManager)] Sound Type not found: {soundType} [ {soundType.ToString()}]");
            return;
        }
        sfxSource.PlayOneShot(clip);
    }
    public void PlayMusic(MusicType musicType)
    {
        if (!MusicByType.TryGetValue(musicType, out AudioClip clip))
        {
            Debug.Log($"[nameof(AudioManager)] Music Type not found: {musicType} [ {musicType.ToString()}]");
            return;
        }
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void SetMasterVolume(float volume)
    {
        float dbvolume = Mathf.Log10(volume) * 20;
        MasterMixer.SetFloat("MasterVolume", dbvolume);
    }
    
    public void SetMusicVolume(float volume)
    {
        float dbvolume = Mathf.Log10(volume) * 20;
        MasterMixer.SetFloat("MusicVolume", dbvolume);
    }
    
    public void SetSFXVolume(float volume)
    {
        float dbvolume = Mathf.Log10(volume) * 20;
        MasterMixer.SetFloat("SFXVolume", dbvolume);
    }
    
    public void SetUIVolume(float volume)
    {
        float dbvolume = Mathf.Log10(volume) * 20;
        MasterMixer.SetFloat("UIVolume", dbvolume);
    }

}
