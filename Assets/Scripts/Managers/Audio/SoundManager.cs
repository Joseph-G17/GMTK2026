using System.Collections;
using System.Collections.Generic;
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

    static Dictionary<AudioSource, Coroutine> activeRepeats = new Dictionary<AudioSource, Coroutine>();

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

    public static void PlaySound(SoundEffect sound, AudioSource sourceOverride, bool repeat, float pitch = 1f)
    {
        if (sound == null || sourceOverride == null) return;

        if (repeat)
        {
            StopRepeating(sourceOverride); 
            Coroutine routine = instance.StartCoroutine(RepeatRoutine(sound, sourceOverride, pitch));
            activeRepeats[sourceOverride] = routine;
        }
        else
        {
            PlayOnce(sound, sourceOverride, pitch);
        }
    }

    static void PlayOnce(SoundEffect sound, AudioSource source, float pitch)
    {
        AudioClip clip = sound.GetRandom();
        if (clip == null) return;

        source.pitch = pitch;
        source.PlayOneShot(clip);
    }

    static IEnumerator RepeatRoutine(SoundEffect sound, AudioSource source, float pitch)
    {
        while (true)
        {
            AudioClip clip = sound.GetRandom();
            if (clip == null) yield break;

            source.pitch = pitch;
            source.PlayOneShot(clip);

            yield return new WaitForSeconds(clip.length / Mathf.Max(pitch, 0.01f));
        }
    }

    public static void StopRepeating(AudioSource source)
    {
        if (activeRepeats.TryGetValue(source, out Coroutine routine))
        {
            if (routine != null)
                instance.StopCoroutine(routine);
            activeRepeats.Remove(source);
        }
    }

    public static void EmitSound(Vector2 position, float radius)
    {
        OnSoundEmitted?.Invoke(position, radius);
    }
}