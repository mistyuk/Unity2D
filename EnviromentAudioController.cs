using UnityEngine;
using System.Collections;

public class EnvironmentAudioMixer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;
    private bool isPlayingFirstSource = true;

    public void PlayAndCrossfade(AudioClip newClip, float fadeDuration)
    {
        if (isPlayingFirstSource)
        {
            StartCoroutine(Crossfade(audioSource1, audioSource2, newClip, fadeDuration));
        }
        else
        {
            StartCoroutine(Crossfade(audioSource2, audioSource1, newClip, fadeDuration));
        }
        isPlayingFirstSource = !isPlayingFirstSource;
    }

    private IEnumerator Crossfade(AudioSource fromSource, AudioSource toSource, AudioClip newClip, float duration)
    {
        // Set up the new clip to play on the toSource
        toSource.clip = newClip;
        toSource.volume = 0f;
        toSource.Play();

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            fromSource.volume = Mathf.Lerp(1f, 0f, timer / duration);
            toSource.volume = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        fromSource.Stop();
        fromSource.volume = 1f; // Reset the volume for future use
    }
}
