using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseOnCollision : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip impactAudio;
    private float volume;
    private Rigidbody rb;
    private bool canMakeNoise;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(canMakeNoise)
        {
            audioSource = GetComponent<AudioSource>() ?? null;
            if (!audioSource)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.pitch = Random.Range(0.8f, 1.0f);
            audioSource.PlayOneShot(impactAudio, volume);
            StartCoroutine(Delay());
        }
    }
    private void Update()
    {
        volume = Mathf.Clamp(rb.velocity.magnitude / 2, 0, 0.5f);
    }
    IEnumerator Delay()
    {
        canMakeNoise = false;
        yield return new WaitForSeconds(0.25f);
        canMakeNoise = true;
    }
}
