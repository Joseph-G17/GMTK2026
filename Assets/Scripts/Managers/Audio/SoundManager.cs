using UnityEngine;
[System.Serializable]
public class SoundEffect
{
    public AudioClip[] clips;

    public AudioClip GetRandom()
    {
        if (clips == null || clips.Length == 0)
            return null;
        return clips[Random.Range(0, clips.Length)];
    }
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public static SoundLibrary Library;

    [SerializeField] SoundLibrary library;
    //detect sounds for enemy
    public delegate void SoundEmittedHandler(Vector2 position, float radius);
    public static event SoundEmittedHandler OnSoundEmitted;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        else
            instance = this;

        Library = library;
    }
    public static void PlaySound(SoundEffect sound, AudioSource sourceOverride, float pitch = 1f)
    {
        if (sound == null || sourceOverride == null) return;
        AudioClip clip = sound.GetRandom();
        if (clip == null) return;

        sourceOverride.pitch = pitch;
        sourceOverride.PlayOneShot(clip);
    }
    public static void EmitSound(Vector2 position, float radius)
    {
        OnSoundEmitted?.Invoke(position, radius);
    }
}
