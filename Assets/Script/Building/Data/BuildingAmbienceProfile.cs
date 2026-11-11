using UnityEngine;

[CreateAssetMenu(fileName = "AmbientAudioProfile", menuName = "Audio/Ambient Audio Profile")]
public class BuildingAmbienceProfile : ScriptableObject
{
    [Header("Audio Clips")]
    public AudioClip[] clips;

    [Header("Playback")]
    public bool loop = true;
    public bool randomizeClips;
    public Vector2 randomInterval = new Vector2(2f, 6f);
    [Range(0f, 1f)] public float volume = 0.5f;

    [Header("3D Audio")]
    public float minDistance = 5f;
    public float maxDistance = 25f;
}