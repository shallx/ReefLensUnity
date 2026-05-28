using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DiveController diveController;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource underwaterSource;
    [SerializeField] private AudioSource swimmingSource;
    [SerializeField] private AudioSource natureSource;

    [Header("Volumes")]
    [SerializeField] private float underwaterVolume = 0.25f;
    [SerializeField] private float swimmingVolume = 1f;
    [SerializeField] private float natureVolume = 0.4f;

    private void Start()
    {
        SetupSource(underwaterSource, underwaterVolume);
        SetupSource(swimmingSource, swimmingVolume);
        SetupSource(natureSource, natureVolume);
    }

    private void Update()
    {
        if (diveController == null || playerMovement == null)
            return;

        bool inWater = diveController.inWater;
        bool atSurface = diveController.isAtSurface;
        bool moving = playerMovement.IsMoving;

        bool playUnderwater = inWater && !atSurface;
        bool playSwimming = inWater && atSurface && moving;
        bool playNature = !inWater;

        HandleSource(underwaterSource, playUnderwater, underwaterVolume);
        HandleSource(swimmingSource, playSwimming, swimmingVolume);
        HandleSource(natureSource, playNature, natureVolume);
    }

    private void SetupSource(AudioSource source, float volume)
    {
        if (source == null) return;

        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.volume = volume;
        source.mute = false;
        source.bypassEffects = true;
        source.bypassListenerEffects = true;
        source.bypassReverbZones = true;
    }

    private void HandleSource(AudioSource source, bool shouldPlay, float volume)
    {
        if (source == null) return;

        source.volume = volume;

        if (shouldPlay && !source.isPlaying)
            source.Play();

        if (!shouldPlay && source.isPlaying)
            source.Stop();
    }
}