using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifePierce : MonoBehaviour
{
    public AudioClip pierceSound;
    public AudioSource audioSource;
    public bool isPiercing = false;
    public float damper;
    public float requiredSpeedToPierce;
    public float limit;
    public Collider knifeCollider;
    public Collider bladeTipCollider;
    public Collider stabbedCollider;
    private ConfigurableJoint joint;
    private Vector3 lastPosition;
    private float velocity;
    public float damage;
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        velocity = distance / Time.deltaTime;
        lastPosition = transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pierceable") && velocity > requiredSpeedToPierce && !isPiercing)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.thisCollider == bladeTipCollider)
                {
                    isPiercing = true;
                    if (collision.transform.GetComponent<JointCollision>())
                    {
                        collision.transform.GetComponent<JointCollision>().npc.DealDamage(damage, 0.5f);
                    }
                    audioSource.pitch = 1f;
                    audioSource.Stop();
                    audioSource.PlayOneShot(pierceSound);

                    stabbedCollider = collision.collider;
                    if (!collision.transform.GetComponent<JointCollision>())
                    {
                        Physics.IgnoreCollision(stabbedCollider, knifeCollider);
                        Physics.IgnoreCollision(stabbedCollider, bladeTipCollider);
                    }
                    else
                    {
                        foreach (Collider collider in collision.transform.GetComponent<JointCollision>().collidersOnNPC)
                        {
                            Physics.IgnoreCollision(collider, knifeCollider);
                            Physics.IgnoreCollision(collider, bladeTipCollider);
                        }
                    }
                    
                    joint = gameObject.AddComponent<ConfigurableJoint>();
                    joint.axis = new Vector3(0, 90, -1);

                    joint.connectedBody = collision.gameObject.GetComponent<Rigidbody>();

                    joint.enableCollision = false;

                    joint.xMotion = ConfigurableJointMotion.Limited;
                    joint.yMotion = ConfigurableJointMotion.Locked;
                    joint.zMotion = ConfigurableJointMotion.Locked;

                    var xDrive = joint.xDrive;
                    xDrive.positionDamper = damper;
                    joint.xDrive = xDrive;

                    var linearLimit = joint.linearLimit;
                    linearLimit.limit = limit;
                    joint.linearLimit = linearLimit;

                    joint.angularXMotion = ConfigurableJointMotion.Locked;
                    joint.angularYMotion = ConfigurableJointMotion.Locked;
                    joint.angularZMotion = ConfigurableJointMotion.Locked;

                    break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Pierceable"))
        {
            if (joint && collision == stabbedCollider)
            {
                if (!collision.transform.GetComponent<JointCollision>())
                {
                    Physics.IgnoreCollision(stabbedCollider, knifeCollider, false);
                    Physics.IgnoreCollision(stabbedCollider, bladeTipCollider, false);
                }
                else
                {
                    foreach (Collider collider in collision.transform.GetComponent<JointCollision>().collidersOnNPC)
                    {
                        Physics.IgnoreCollision(collider, knifeCollider, false);
                        Physics.IgnoreCollision(collider, bladeTipCollider, false);
                    }
                }

                stabbedCollider = null;
                StartCoroutine(Delay());
                audioSource.pitch = 1.5f;
                audioSource.Stop();
                audioSource.PlayOneShot(pierceSound);
                Destroy(joint);
            }
        }
    }
    IEnumerator Delay()
    {
        knifeCollider.enabled = false;
        bladeTipCollider.enabled = false;
        yield return new WaitForSeconds(0.25f);
        knifeCollider.enabled = true;
        bladeTipCollider.enabled = true;
        isPiercing = false;
    }
}
