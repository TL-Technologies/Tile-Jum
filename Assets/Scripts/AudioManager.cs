using System;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public static event Action<bool> SoundStateChanged; 
   

    public static bool IsSoundEnable
    {
        get { return PlayerPrefs.GetInt(nameof(IsSoundEnable), 1) == 1; }
        set
        {
            if (value == IsSoundEnable)
            {
                return;
            }

            PlayerPrefs.SetInt(nameof(IsSoundEnable),value?1:0);
            SoundStateChanged?.Invoke(value);
        }
    }

    public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position)
    {
        var clipGo = new GameObject("Clip");
        clipGo.transform.position = position;
        var audioSource = clipGo.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
        if (clipGo != null)
        {
            Destroy(clipGo,clip.length);
        }
        
        return audioSource;
    }
}