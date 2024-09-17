using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; set; }
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        PlayMusic("Background Music", 0.4f); 
    }

    public void PlayMusic(string name, float volume)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.volume = Mathf.Clamp(volume, 0f, 1f);
            musicSource.Play();
        }
    }

    public void PlaySFX(string name, float volume)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.volume = Mathf.Clamp(volume, 0f, 1f);
            sfxSource.PlayOneShot(s.clip);
        }
    }

    // Play a sound effect only once
    public void PlayOneShot(string name, float volume)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.volume = Mathf.Clamp(volume, 0f, 1f);
            sfxSource.PlayOneShot(s.clip);
        }
    }
}
