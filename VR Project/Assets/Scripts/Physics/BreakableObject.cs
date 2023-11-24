using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BreakableObject : MonoBehaviour
{
    public GameObject breakableParent;
    private List<Rigidbody> breakables;
    public float forceNeededToBreak;
    public AudioSource audioSource;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude > 5f)
        {
            forceNeededToBreak -= collision.relativeVelocity.magnitude;
        }
        if(forceNeededToBreak < 0 )
        {
            Break(collision.relativeVelocity.magnitude * 2, collision.relativeVelocity);
        }
    }
    public void Break(float breakForce, Vector3 velocity)
    {
        GetComponent<XRGrabInteractable>().enabled = false;
        breakableParent.SetActive(true);
        audioSource.Play();
        breakableParent.transform.parent = null;
        gameObject.SetActive(false);
        breakables = breakableParent.GetComponentsInChildren<Rigidbody>().ToList();
        foreach (Rigidbody rb in breakables)
        {
            rb.AddExplosionForce(breakForce, transform.position, 100);
            rb.AddForce(velocity * 10);
        }
    }
}
