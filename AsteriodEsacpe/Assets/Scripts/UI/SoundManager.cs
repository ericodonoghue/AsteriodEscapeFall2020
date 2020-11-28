using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundStates { Start, Stop }
public enum ScaryNoises
{
      Jet
    , Heartbeat50bpm
    , Heartbeat60bpm
    , Heartbeat70bpm
    , Heartbeat80bpm
    , Heartbeat90bpm
    , Heartbeat100bpm
    , Heartbeat110bpm
    , Heartbeat120bpm
    , Heartbeat130bpm
    , Heartbeat140bpm
    , Heartbeat150bpm
    , Heartbeat160bpm
    , Heartbeat170bpm
    , Heartbeat180bpm
    , Heartbeat190bpm
    , Heartbeat200bpm
    , Gasping1
    , Gasping2
    , Gasping3
    , Gasping4
    , Impact1
    , Impact2
    , Impact3
    , Impact4
    , AirLeak1
    , AirLeak2
    , AirLeak3
    , AirLeak4
}

public class SoundManager : MonoBehaviour
{
    #region Public Fields

    public AudioSource jetSound;
    public AudioSource Heartbeat50bpm;
    public AudioSource Heartbeat60bpm;
    public AudioSource Heartbeat70bpm;
    public AudioSource Heartbeat80bpm;
    public AudioSource Heartbeat90bpm;
    public AudioSource Heartbeat100bpm;
    public AudioSource Heartbeat110bpm;
    public AudioSource Heartbeat120bpm;
    public AudioSource Heartbeat130bpm;
    public AudioSource Heartbeat140bpm;
    public AudioSource Heartbeat150bpm;
    public AudioSource Heartbeat160bpm;
    public AudioSource Heartbeat170bpm;
    public AudioSource Heartbeat180bpm;
    public AudioSource Heartbeat190bpm;
    public AudioSource Heartbeat200bpm;
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
            ,{ ScaryNoises.Heartbeat50bpm, this.Heartbeat50bpm }
            ,{ ScaryNoises.Heartbeat60bpm, this.Heartbeat60bpm }
            ,{ ScaryNoises.Heartbeat70bpm, this.Heartbeat70bpm }
            ,{ ScaryNoises.Heartbeat80bpm, this.Heartbeat80bpm }
            ,{ ScaryNoises.Heartbeat90bpm, this.Heartbeat90bpm }
            ,{ ScaryNoises.Heartbeat100bpm, this.Heartbeat100bpm }
            ,{ ScaryNoises.Heartbeat110bpm, this.Heartbeat110bpm }
            ,{ ScaryNoises.Heartbeat120bpm, this.Heartbeat120bpm }
            ,{ ScaryNoises.Heartbeat130bpm, this.Heartbeat130bpm }
            ,{ ScaryNoises.Heartbeat140bpm, this.Heartbeat140bpm }
            ,{ ScaryNoises.Heartbeat150bpm, this.Heartbeat150bpm }
            ,{ ScaryNoises.Heartbeat160bpm, this.Heartbeat160bpm }
            ,{ ScaryNoises.Heartbeat170bpm, this.Heartbeat170bpm }
            ,{ ScaryNoises.Heartbeat180bpm, this.Heartbeat180bpm }
            ,{ ScaryNoises.Heartbeat190bpm, this.Heartbeat190bpm }
            ,{ ScaryNoises.Heartbeat200bpm, this.Heartbeat200bpm }

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
