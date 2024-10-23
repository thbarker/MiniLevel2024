using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController musicController;

    [SerializeField]
    private AudioClip menuMusic;
    [SerializeField]
    private AudioClip gameplayMusic;
    [SerializeField]
    private AudioSource audioSource1, audioSource2;
    [SerializeField]
    private float fadeDuration = 1f;

    void Awake()
    {
        if (musicController != null)
        {
            Destroy(gameObject);
            return;
        }

        musicController = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        audioSource1.PlayOneShot(menuMusic);
    }

    public void SwitchToGameplayMusic()
    {
        StartCoroutine(Crossfade(audioSource1, audioSource2, gameplayMusic));
    }

    private IEnumerator Crossfade(AudioSource currentSource, AudioSource newSource, AudioClip newClip)
    {
        float elapsedTime = 0f;

        // Prepare new audio source
        newSource.clip = newClip;
        newSource.volume = 0f;  // Start with volume at 0 for the new source
        newSource.Play();

        while (elapsedTime < fadeDuration)
        {
            // Fade out the current source
            currentSource.volume = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            // Fade in the new source
            newSource.volume = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure volumes are fully transitioned
        currentSource.volume = 0f;
        newSource.volume = 1f;

        // Stop the current (faded out) source
        currentSource.Stop();
    }

}