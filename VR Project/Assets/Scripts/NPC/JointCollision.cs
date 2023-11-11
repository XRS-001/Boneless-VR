using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointCollision : MonoBehaviour
{
    public NPC npc;
    public float velocityThreshold;
    private bool canCollide = false;
    public bool damageOnCollision = true;
    public Collider[] collidersOnNPC;
    private void Start()
    {
        StartCoroutine(Delay());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(damageOnCollision && collision.gameObject.layer != 10)
        {
            if (collision.relativeVelocity.magnitude > velocityThreshold && canCollide)
            {
                if (collision.gameObject.layer != 14 && collision.gameObject.layer != 15 && collision.gameObject.layer != 16 && collision.gameObject.layer != 20 && collision.gameObject.layer != 8 && collision.gameObject.layer != 9)
                {
                    npc.DealDamage(collision.relativeVelocity.magnitude / 4);
                    StartCoroutine(Delay());
                }
            }
            if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
            {
                if (collision.rigidbody.velocity.magnitude > velocityThreshold * 2)
                {
                    npc.DealDamage(collision.relativeVelocity.magnitude);
                    StartCoroutine(Delay());
                }
            }
        }
        if(collision.gameObject.layer == 10)
        {
            npc.DealDamage(collision.relativeVelocity.magnitude / 4);
        }
    }
    public IEnumerator Delay()
    {
        canCollide = false;
        yield return new WaitForSeconds(1);
        canCollide = true;
    }
}
