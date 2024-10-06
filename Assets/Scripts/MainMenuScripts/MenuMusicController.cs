using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] backgroundMusicOptions;

    [SerializeField] GameObject audioSourceController;

    private int index = 0;

    private void Start()
    {
        audioSource.clip = backgroundMusicOptions[index];
        audioSource.Play();
    }

    public void SwitchMusicRight()
    {
        index++;
        if (index >= backgroundMusicOptions.Length)
        {
            index = 0;
        }
        audioSource.clip = backgroundMusicOptions[index];
        audioSource.Play();
    }

    public void SwitchMusicLeft()
    {
        index--;
        if (index < 0)
        {
            index = backgroundMusicOptions.Length - 1;
        }
        audioSource.clip = backgroundMusicOptions[index];
        audioSource.Play();
    }

}
