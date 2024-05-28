using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public AudioSource sfxSource;
    public AudioClip[] sfxClips;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void PlaySFX(int clipIndex)
    {
        sfxSource.PlayOneShot(sfxClips[clipIndex]);
    }
    
    public void PlaySFX(string clipName)
    {
        var c = sfxClips.Where(clip => clip.name == clipName).ToList();
        if (c.Count > 0) sfxSource.PlayOneShot(c[0]);
    }
    
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
