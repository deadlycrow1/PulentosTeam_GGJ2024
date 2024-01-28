using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXHelper : MonoBehaviour
{
    public AudioClip[] FXs;
    public AudioClip[] steps;
    public AudioSource stepsAudio;
    public float stepsVolume = 0.3f;

    public void Footstep(AnimationEvent evt) {
        if(evt.animatorClipInfo.weight > 0.4f) {
            stepsAudio.PlayOneShot(steps[Random.Range(0, steps.Length)], stepsVolume);
        }
    }
}
