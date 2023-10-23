using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseOnCollision : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip impactAudio;
    private float volume;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        audioSource = GetComponent<AudioSource>() ?? null;
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.pitch = Random.Range(0.8f, 1.0f);
        audioSource.PlayOneShot(impactAudio, volume);
    }
    private void Update()
    {
        volume = Mathf.Clamp(rb.velocity.magnitude, 0, 0.5f);
    }
}
