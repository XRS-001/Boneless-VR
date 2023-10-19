using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPresence : MonoBehaviour
{
    public Rigidbody body;
    public bool isColliding;
    private bool collided;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collided");
        if (collision.gameObject.CompareTag("Interactable"))
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
