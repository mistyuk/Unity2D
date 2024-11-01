using UnityEngine;
using System.Collections;


public class BackgroundMusicController : MonoBehaviour
{
    [SerializeField] private AudioSource musicAudioSource; // The AudioSource for playing music

    // Play a music track
    public void PlayMusic(AudioClip musicClip, float volume = 1.0f, bool loop = true)
    {
        if (musicAudioSource == null)
        {
            Debug.LogError("BackgroundMusicController: AudioSource is not assigned.");
            return;
        }

        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop(); // Stop the currently playing music if needed
        }

        musicAudioSource.clip = musicClip;
        musicAudioSource.volume = volume;
        musicAudioSource.loop = loop;
        musicAudioSource.Play();
    }

    // Stop the current music
    public void StopMusic()
    {
        if (musicAudioSource != null && musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }
    }

    // Fade out current music over a specified duration
    public void FadeOutMusic(float fadeDuration)
    {
        StartCoroutine(FadeOutCoroutine(fadeDuration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = musicAudioSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        musicAudioSource.Stop();
        musicAudioSource.volume = startVolume; // Reset volume for future use
    }
}
