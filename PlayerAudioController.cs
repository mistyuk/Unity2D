// Assets/Scripts/PlayerAudioController.cs
using UnityEngine;
using UnityEngine.Audio; // For routing audio to mixers
using System.Collections;
using System.Collections.Generic;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource jumpAudioSource;
    [SerializeField] private AudioSource airtimeAudioSource; // Renamed from windAudioSource
    [SerializeField] private AudioSource crouchAudioSource;
    [SerializeField] private AudioSource takeDamageAudioSource;
    [SerializeField] private AudioSource deathAudioSource;
    [SerializeField] private AudioSource landAudioSource;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource attackAudioSource; // New AudioSource for attack sound

    // Audio clips and settings for various actions (can have multiple clips)
    [System.Serializable]
    public class SoundSettings
    {
        public List<AudioClip> clips;
        public bool avoidRepeats = true;
        [HideInInspector] public AudioClip lastClipPlayed;
    }

    [Header("Audio Clips and Settings")]
    [SerializeField] private SoundSettings jumpSounds;
    [SerializeField] private SoundSettings airtimeSounds;
    [SerializeField] private SoundSettings crouchSounds;
    [SerializeField] private SoundSettings takeDamageSounds;
    [SerializeField] private SoundSettings deathSounds;
    [SerializeField] private SoundSettings landSounds;
    [SerializeField] private SoundSettings footstepSounds;
    [SerializeField] private SoundSettings attackSounds;

    [SerializeField] private AudioMixer playerAudioMixer;
    [SerializeField] private string jumpMixerGroup = "JumpGroup";
    [SerializeField] private string airtimeMixerGroup = "AirtimeGroup"; // Renamed from windMixerGroup
    [SerializeField] private string crouchMixerGroup = "CrouchGroup";
    [SerializeField] private string takeDamageMixerGroup = "TakeDamageGroup";
    [SerializeField] private string deathMixerGroup = "DeathGroup";
    [SerializeField] private string landMixerGroup = "LandGroup";
    [SerializeField] private string footstepMixerGroup = "FootstepGroup";
    [SerializeField] private string attackMixerGroup = "AttackGroup"; // New mixer group for attack sound

    [SerializeField] private PlayerController2D playerController2D; // Assign via Inspector
    private float stepTimer;

    public float baseStepInterval = 0.5f;
    public float velocityThreshold = 0.1f; // Minimum velocity required to play footstep sounds

    private Rigidbody2D playerRigidbody;

    void Start()
    {
        // If playerController2D is not assigned in the Inspector, try to get it from the same GameObject
        if (playerController2D == null)
        {
            playerController2D = GetComponent<PlayerController2D>();
            if (playerController2D == null)
            {
                // Try to find PlayerController2D in the scene
                playerController2D = FindObjectOfType<PlayerController2D>();
                if (playerController2D == null)
                {
                    Debug.LogError("PlayerAudioController: Could not find PlayerController2D. Please assign it in the inspector.");
                    return;
                }
            }
        }

        // Get the player's Rigidbody2D
        playerRigidbody = playerController2D.GetComponent<Rigidbody2D>();
        if (playerRigidbody == null)
        {
            Debug.LogError("PlayerAudioController: Could not find Rigidbody2D on the player.");
            return;
        }

        // Check AudioSources
        if (jumpAudioSource == null || airtimeAudioSource == null || crouchAudioSource == null || takeDamageAudioSource == null ||
            deathAudioSource == null || landAudioSource == null || footstepAudioSource == null || attackAudioSource == null)
        {
            Debug.LogError("Missing references to AudioSources. Please assign them in the inspector.");
            return;
        }

        // Assign mixer groups to audio sources
        AssignMixerGroup(jumpAudioSource, jumpMixerGroup);
        AssignMixerGroup(airtimeAudioSource, airtimeMixerGroup);
        AssignMixerGroup(crouchAudioSource, crouchMixerGroup);
        AssignMixerGroup(takeDamageAudioSource, takeDamageMixerGroup);
        AssignMixerGroup(deathAudioSource, deathMixerGroup);
        AssignMixerGroup(landAudioSource, landMixerGroup);
        AssignMixerGroup(footstepAudioSource, footstepMixerGroup);
        AssignMixerGroup(attackAudioSource, attackMixerGroup);

        // Set airtimeAudioSource settings
        airtimeAudioSource.loop = true;
        airtimeAudioSource.volume = 0f; // Start with volume at 0

        // Subscribe to PlayerController2D events
        playerController2D.OnJump.AddListener(StartAirtimeSound);
        playerController2D.OnLand.AddListener(StopAirtimeSound);
        playerController2D.OnCrouch.AddListener(PlayCrouchSound);
        playerController2D.OnTakeDamage.AddListener(PlayTakeDamageSound);
        playerController2D.OnDeath.AddListener(PlayDeathSound);
        playerController2D.OnAttack.AddListener(PlayAttackSound); // Subscribe to attack event

        // Optional: Subscribe to OnFall if needed
        // playerController2D.OnFall.AddListener(PlayFallSound);
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (playerController2D != null)
        {
            playerController2D.OnJump.RemoveListener(StartAirtimeSound);
            playerController2D.OnLand.RemoveListener(StopAirtimeSound);
            playerController2D.OnCrouch.RemoveListener(PlayCrouchSound);
            playerController2D.OnTakeDamage.RemoveListener(PlayTakeDamageSound);
            playerController2D.OnDeath.RemoveListener(PlayDeathSound);
            playerController2D.OnAttack.RemoveListener(PlayAttackSound); // Unsubscribe from attack event
        }
    }

    void Update()
    {
        HandleFootsteps();
        HandleAirtimeSoundVolume();
    }

    // Handle footstep sounds
   void HandleFootsteps()
{
    if (playerController2D != null && playerController2D.isGrounded && playerController2D.GetVelocity().magnitude > velocityThreshold)
    {
        // Check if the player is not crouching
        if (!playerController2D.isCrouching) // Assuming 'isCrouching' is a boolean in PlayerController2D
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                PlayFootstepSound();
                stepTimer = baseStepInterval;
            }
        }
    }
}


    // Handle airtime sound volume adjustments
    void HandleAirtimeSoundVolume()
    {
        if (airtimeAudioSource.isPlaying)
        {
            float verticalVelocity = playerRigidbody.velocity.y;

            // Calculate the normalized ascent and descent
            float normalizedAscent = Mathf.InverseLerp(0, playerController2D.jumpForce, verticalVelocity);
            float normalizedDescent = Mathf.InverseLerp(0, -playerController2D.maxFallSpeed, verticalVelocity);

            float targetVolume = 0f;

            if (verticalVelocity > 0)
            {
                // Ascending: Increase volume from 0 to max
                targetVolume = Mathf.Lerp(0f, 1f, 1f - normalizedAscent);
            }
            else if (verticalVelocity < 0)
            {
                // Descending: Decrease volume from max to 0
                targetVolume = Mathf.Lerp(1f, 0f, normalizedDescent);
            }

            // Smoothly adjust the volume
            airtimeAudioSource.volume = Mathf.Lerp(airtimeAudioSource.volume, targetVolume, Time.deltaTime * 5f);
        }
    }

    // Methods to play sounds for different actions
    void StartAirtimeSound()
    {
        PlayJumpSound(); // Play the jump sound

        if (!airtimeAudioSource.isPlaying)
        {
            // Select a random airtime clip
            AudioClip clip = GetRandomClip(airtimeSounds);
            if (clip != null)
            {
                airtimeAudioSource.clip = clip;
                airtimeAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("No airtime clips assigned.");
            }
        }
    }

    void StopAirtimeSound()
    {
        PlayLandSound(); // Play the landing sound

        // Fade out the airtime sound
        StartCoroutine(FadeOutAirtimeSound());
    }

    IEnumerator FadeOutAirtimeSound()
    {
        float startVolume = airtimeAudioSource.volume;

        while (airtimeAudioSource.volume > 0f)
        {
            airtimeAudioSource.volume -= startVolume * Time.deltaTime * 5f;
            yield return null;
        }

        airtimeAudioSource.Stop();
        airtimeAudioSource.volume = 0f;
    }

    void PlayAttackSound()
    {
        PlayRandomSound(attackAudioSource, attackSounds, "attack");
    }

    void PlayCrouchSound()
    {
        PlayRandomSound(crouchAudioSource, crouchSounds, "crouch");
    }

    void PlayTakeDamageSound()
    {
        PlayRandomSound(takeDamageAudioSource, takeDamageSounds, "take damage");
    }

    void PlayDeathSound()
    {
        PlayRandomSound(deathAudioSource, deathSounds, "death");
    }

    void PlayFootstepSound()
    {
        PlayRandomSound(footstepAudioSource, footstepSounds, "footstep");
    }

    void PlayLandSound()
    {
        PlayRandomSound(landAudioSource, landSounds, "land");
    }

    void PlayJumpSound()
    {
        PlayRandomSound(jumpAudioSource, jumpSounds, "jump");
    }

    void PlayRandomSound(AudioSource audioSource, SoundSettings soundSettings, string actionName)
    {
        AudioClip clip = GetRandomClip(soundSettings);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"No audio clips assigned for {actionName}.");
        }
    }

    AudioClip GetRandomClip(SoundSettings soundSettings)
    {
        if (soundSettings.clips != null && soundSettings.clips.Count > 0)
        {
            if (soundSettings.clips.Count == 1)
            {
                return soundSettings.clips[0];
            }
            else
            {
                AudioClip newClip;
                int attempts = 0;
                do
                {
                    int index = Random.Range(0, soundSettings.clips.Count);
                    newClip = soundSettings.clips[index];
                    attempts++;
                }
                while (soundSettings.avoidRepeats && newClip == soundSettings.lastClipPlayed && attempts < 10);

                soundSettings.lastClipPlayed = newClip;
                return newClip;
            }
        }
        return null;
    }

    void AssignMixerGroup(AudioSource audioSource, string mixerGroupName)
    {
        if (playerAudioMixer != null && !string.IsNullOrEmpty(mixerGroupName))
        {
            AudioMixerGroup[] groups = playerAudioMixer.FindMatchingGroups(mixerGroupName);
            if (groups.Length > 0)
            {
                audioSource.outputAudioMixerGroup = groups[0];
            }
            else
            {
                Debug.LogWarning($"Mixer group '{mixerGroupName}' not found for {audioSource.name}.");
            }
        }
    }
}
