using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BreakableObject : MonoBehaviour
{
    public GameObject breakableParent;
    private List<Rigidbody> breakables;
    public float speedNeededToBreak;
    public float velocity;
    private float breakForce;
    public AudioSource audioSource;
    private void Update()
    {
        velocity = GetComponent<Rigidbody>().velocity.magnitude;
        breakForce = velocity * 75;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collided");
        if (velocity > speedNeededToBreak || (collision?.rigidbody?.velocity.magnitude > speedNeededToBreak && collision?.rigidbody?.mass > 1))
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
            }
        }
    }
}
