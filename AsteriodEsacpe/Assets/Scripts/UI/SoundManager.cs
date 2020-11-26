using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundStates { Start, Stop }
public enum ScaryNoises
{
      Jet
    , Pulse1
    , Pulse2
    , Pulse3
    , Pulse4
    , Gasping1
    , Gasping2
    , Gasping3
    , Gasping4
    , Impact1
    , Impact2
    , Impact3
    , Impact4
}

public class SoundManager : MonoBehaviour
{
    #region Public Fields

    public AudioSource jetSound;
    public AudioSource pulse1Sound;
    public AudioSource pulse2Sound;
    public AudioSource pulse3Sound;
    public AudioSource pulse4Sound;
    public AudioSource gasping1Sound;
    public AudioSource gasping2Sound;
    public AudioSource gasping3Sound;
    public AudioSource gasping4Sound;
    public AudioSource impact1Sound;
    public AudioSource impact2Sound;
    public AudioSource impact3Sound;
    public AudioSource impact4Sound;

    #endregion Public Fields

    Dictionary<ScaryNoises, AudioSource> noiseMap;

    private void Awake()
    {
        // Create a type to sound mapping for programmatic sound management without if-else or switch blocks
        noiseMap = new Dictionary<ScaryNoises, AudioSource>
        {
             { ScaryNoises.Jet, this.jetSound }
            ,{ ScaryNoises.Pulse1, this.pulse1Sound }
            ,{ ScaryNoises.Pulse2, this.pulse2Sound }
            ,{ ScaryNoises.Pulse3, this.pulse3Sound }
            ,{ ScaryNoises.Pulse4, this.pulse4Sound }
            ,{ ScaryNoises.Gasping1, this.gasping1Sound }
            ,{ ScaryNoises.Gasping2, this.gasping2Sound }
            ,{ ScaryNoises.Gasping3, this.gasping3Sound }
            ,{ ScaryNoises.Gasping4, this.gasping4Sound }
            ,{ ScaryNoises.Impact1, this.impact1Sound }
            ,{ ScaryNoises.Impact2, this.impact2Sound }
            ,{ ScaryNoises.Impact3, this.impact3Sound }
            ,{ ScaryNoises.Impact4, this.impact4Sound }
        };
    }

    public void SetSoundState(SoundStates state, ScaryNoises noise, float volume = 1f, float playbackRate = 1f)
    {
        // Get audio source from mapping
        AudioSource audioSource = this.noiseMap[noise];


        // Only try to use the audio source if it is configured
        if (audioSource != null)
        {
            if (state == SoundStates.Start)
            {
                if (!audioSource.isPlaying)
                {
                    // Not sure how to do this, but would be really neat:
                    // TODO: playbackRate

                    audioSource.volume = volume;
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.Stop();
            }
        }
    }
}
