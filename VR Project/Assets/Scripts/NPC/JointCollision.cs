using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointCollision : MonoBehaviour
{
    public NPC npc;
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
            if(collision.gameObject.layer != 14 && collision.gameObject.layer != 15 && collision.gameObject.layer != 16 && collision.gameObject.layer != 20 && collision.gameObject.layer != 8 && collision.gameObject.layer != 9)
            {
                npc.DealDamage(collision.relativeVelocity.magnitude / 2);
                StartCoroutine(Delay());
            }
        }
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
        {
            if(collision.rigidbody.velocity.magnitude > velocityThreshold * 2)
            {
                Debug.Log(collision.rigidbody.velocity.magnitude);
                npc.DealDamage(collision.relativeVelocity.magnitude / 2);
                StartCoroutine(Delay());
            }
        }
    }
    public IEnumerator Delay()
    {
        canCollide = false;
        yield return new WaitForSeconds(1);
        canCollide = true;
    }
}
