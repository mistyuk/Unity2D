using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnvironmentSoundSettings
{
    public List<AudioClip> clips;
    public float minInterval = 5.0f; // Minimum interval between plays
    public float maxInterval = 15.0f; // Maximum interval between plays
    public float volume = 1.0f;
}

public class EnvironmentalSoundController : MonoBehaviour
{
    [Header("Environmental Sound Settings")]
    [SerializeField] private AudioSource atmosphereAudioSource;
    [SerializeField] private AudioSource windAudioSource;
    [SerializeField] private AudioSource animalAudioSource;

    [SerializeField] private EnvironmentSoundSettings atmosphereSettings;
    [SerializeField] private EnvironmentSoundSettings windSettings;
    [SerializeField] private EnvironmentSoundSettings animalSettings;

    void Start()
    {
        // Start the playback for looping and interval-based sounds
        if (atmosphereSettings.clips.Count > 0)
        {
            StartCoroutine(PlayLoopingSound(atmosphereAudioSource, atmosphereSettings, true));
        }
        if (windSettings.clips.Count > 0)
        {
            StartCoroutine(PlayIntervalSound(windAudioSource, windSettings));
        }
        if (animalSettings.clips.Count > 0)
        {
            StartCoroutine(PlayIntervalSound(animalAudioSource, animalSettings));
        }
    }

    private IEnumerator PlayLoopingSound(AudioSource source, EnvironmentSoundSettings settings, bool loop = false)
    {
        source.clip = settings.clips[Random.Range(0, settings.clips.Count)];
        source.volume = settings.volume;
        source.loop = loop;
        source.Play();

        // Return null if the sound should loop indefinitely
        if (loop) yield return null;
        else yield return new WaitForSeconds(source.clip.length);
    }

    private IEnumerator PlayIntervalSound(AudioSource source, EnvironmentSoundSettings settings)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(settings.minInterval, settings.maxInterval));

            if (!source.isPlaying)
            {
                source.clip = settings.clips[Random.Range(0, settings.clips.Count)];
                source.volume = settings.volume;
                source.Play();
            }
        }
    }
}
