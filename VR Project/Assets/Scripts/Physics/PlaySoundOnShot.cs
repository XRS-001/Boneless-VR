using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnShot : MonoBehaviour
{
    private bool canMakeNoise = true;
    public AudioSource audioSource;
    public AudioClip hitSound;
    private void OnCollisionEnter(Collision collision)
    {
        if (canMakeNoise)
        {
            if (audioSource.enabled)
            {
                audioSource.PlayOneShot(hitSound);
                Destroy(gameObject);
            }
        }
        canMakeNoise = false;
    }
}
