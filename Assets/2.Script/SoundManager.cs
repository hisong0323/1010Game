using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip pickSound;

    [SerializeField]
    private AudioClip dropSound;

    [SerializeField]
    private AudioClip buttonSound;

    public static SoundManager Instance { get; private set; }

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySFX(AudioClip clip, float volum = 1)
    {
        _audioSource.PlayOneShot(clip, volum);
    }

    public void PlayPickSound()
    {
        _audioSource.PlayOneShot(pickSound, 0.2f);
    }

    public void PlayDropSound()
    {
        _audioSource.PlayOneShot(dropSound, 0.7f);
    }

    public void PlayButtonSound()
    {
        _audioSource.PlayOneShot(buttonSound);
    }
}
