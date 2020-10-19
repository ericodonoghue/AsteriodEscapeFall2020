using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingScript : MonoBehaviour
{
    private PostProcessVolume volume;
    private Vignette vignette;
    private AvatarAccounting avatarAccounting;

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        volume.sharedProfile.TryGetSettings(out vignette);
        avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();
    }

    // Update is called once per frame
    void Update()
    {
        if (vignette != null)
        {        
            float intensity = CalculateVignetteIntensity();
            vignette.intensity.value = intensity;
        }
    }

    private float CalculateVignetteIntensity ()
    {
        float currO2 = avatarAccounting.CurrentOxygenAllTanksContent;
        return 1.0f - (currO2*0.0001f);
    }
}
