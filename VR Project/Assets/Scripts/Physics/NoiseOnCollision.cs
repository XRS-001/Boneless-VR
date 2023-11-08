using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseOnCollision : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip impactAudio;
    private float volume;
    private bool canMakeNoise;
    private void OnCollisionEnter(Collision collision)
    {
        if(canMakeNoise)
        {
            audioSource = GetComponent<AudioSource>() ?? null;
            if (!audioSource)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            volume = Mathf.Clamp(collision.relativeVelocity.magnitude, 0, 0.3f);
            audioSource.pitch = Random.Range(0.6f, 0.8f);
            audioSource.PlayOneShot(impactAudio, volume);
            StartCoroutine(Delay());
        }
    }
    IEnumerator Delay()
    {
        canMakeNoise = false;
        yield return new WaitForSeconds(0.25f);
        canMakeNoise = true;
    }
}
