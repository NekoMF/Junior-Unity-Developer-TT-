using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManger : MonoBehaviour
{
    public static SoundManger Instance{get; set; }

    public AudioSource shootingSoundAK47;
    public AudioSource reloadingSoundAK47; 
    public AudioSource emptyMagazineSound; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }else
        {
            Instance = this;
        }
    }
}
