using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointCollision : MonoBehaviour
{
    public float velocityThreshold;
    private bool canCollide = false;
    private void Start()
    {
        StartCoroutine(Delay());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude > velocityThreshold && canCollide)
        {
            transform.root.gameObject.GetComponent<NPC>().DealDamage(collision.relativeVelocity.magnitude / 2);
        }
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);
        canCollide = true;
    }
}
