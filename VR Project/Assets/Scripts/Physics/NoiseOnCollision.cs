using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseOnCollision : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip impactAudio;
    private float volume;
    private bool canMakeNoise = true;
    private void OnCollisionEnter(Collision collision)
    {
        if(canMakeNoise)
        {
            audioSource = GetComponent<AudioSource>() ?? null;
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1;
            }
            if(!audioSource.isPlaying)
            {
                volume = Mathf.Clamp(collision.relativeVelocity.magnitude, 0, 0.1f);
                audioSource.pitch = Random.Range(0.8f, 1f);
                audioSource.PlayOneShot(impactAudio, volume);
                StartCoroutine(Delay());
            }
        }
    }
    IEnumerator Delay()
    {
        canMakeNoise = false;
        yield return new WaitForSeconds(0.25f);
        canMakeNoise = true;
    }
}
