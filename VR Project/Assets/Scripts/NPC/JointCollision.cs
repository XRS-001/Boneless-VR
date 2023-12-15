using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointCollision : MonoBehaviour
{
    public GameObject bloodDecal;
    public float bulletDamage;
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
        if(damageOnCollision && collision.gameObject.layer != 10 && collision.gameObject.layer != 6)
        {
            if (collision.relativeVelocity.magnitude > velocityThreshold / 8 && canCollide)
            {
                if (collision.gameObject.layer != 14 && collision.gameObject.layer != 15 && collision.gameObject.layer != 16 && collision.gameObject.layer != 20 && collision.gameObject.layer != 8 && collision.gameObject.layer != 9)
                {
                    npc.DealDamage(collision.relativeVelocity.magnitude * 4, 0.5f);
                    StartCoroutine(Delay());
                }
            }
            if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
            {
                if (collision.rigidbody.velocity.magnitude > velocityThreshold)
                {
                    npc.DealDamage(collision.relativeVelocity.magnitude, 0.5f);
                    StartCoroutine(Delay());
                }
            }
        }
        if(collision.gameObject.layer == 10)
        {
            npc.DealDamage(bulletDamage, 0);
            if (npc.health == 0)
            {
                collision.rigidbody.AddForce(collision.relativeVelocity * 25, ForceMode.Impulse);
            }
            canCollide = true;
        }
        if (collision.gameObject.layer == 6)
        {
            if (collision.gameObject.GetComponent<KnifeSlice>())
            {
                if (collision.rigidbody.velocity.magnitude > collision.gameObject.GetComponent<KnifeSlice>().speedNeededToSlice)
                {
                    StartCoroutine(CheckNotPierce(collision.gameObject.GetComponent<KnifePierce>(), collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal)));
                }
            }
            else if (collision.rigidbody.velocity.magnitude > velocityThreshold)
            {
                GameObject spawnedDecal = Instantiate(bloodDecal, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
                spawnedDecal.transform.parent = transform;
            }
        }

    }
    public IEnumerator CheckNotPierce(KnifePierce knifePierce, Vector3 position, Quaternion rotation)
    {
        yield return new WaitForSeconds(0.001f);
        if (!knifePierce.isPiercing)
        {
            GameObject spawnedDecal = Instantiate(bloodDecal, position, rotation);
            spawnedDecal.transform.parent = transform;
        }
    }
    public void BloodWound(KnifePierce knifePierce, Vector3 position, Quaternion rotation)
    {
        GameObject spawnedDecal = Instantiate(bloodDecal, position, rotation);
        spawnedDecal.transform.parent = transform;
    }
    public IEnumerator Delay()
    {
        canCollide = false;
        yield return new WaitForSeconds(1);
        canCollide = true;
    }
}
