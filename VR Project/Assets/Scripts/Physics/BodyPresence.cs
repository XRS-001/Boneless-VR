using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BodyPresence : MonoBehaviour
{
    public Rigidbody body;
    public bool isColliding;
    private bool collided;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Interactable") && collision.gameObject.GetComponent<XRBaseInteractable>().isSelected)
        {
            body.isKinematic = true;
            isColliding = true;
            collided = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            if(isColliding)
            {
                isColliding = false;
                StartCoroutine(Delay());
            }
        }
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);

        if (!isColliding && collided)
        {
            body.isKinematic = false;
            collided = false;
        }
    }
}
