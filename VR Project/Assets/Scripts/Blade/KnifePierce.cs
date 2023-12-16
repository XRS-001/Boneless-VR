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
    public Transform pierce;
    private Collider stabbedCollider;
    private ConfigurableJoint joint;
    private Vector3 lastPosition;
    private float velocity;
    public float damage;
    private Transform hitPoint;
    private GameObject hitObject;
    private void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        velocity = distance / Time.deltaTime;
        lastPosition = transform.position;

        if(Physics.CheckSphere(pierce.position, 0.025f) && !isPiercing)
        {
            stabbedCollider = Physics.OverlapSphere(pierce.position, 0.025f)[0];

            if ((stabbedCollider.CompareTag("Pierceable") || stabbedCollider.transform.root.CompareTag("Pierceable")) && velocity > requiredSpeedToPierce && !isPiercing)
            {
                hitObject = new GameObject();
                hitPoint = hitObject.transform;
                hitObject.transform.parent = stabbedCollider.transform;
                hitPoint.position = pierce.position + (-pierce.forward * 0.035f);
                isPiercing = true;
                if (stabbedCollider.transform.GetComponent<JointCollision>())
                {
                    stabbedCollider.transform.GetComponent<JointCollision>().BloodWound(this, pierce.position, pierce.rotation);
                    stabbedCollider.transform.GetComponent<JointCollision>().npc.DealDamage(damage, 0.5f);
                }
                audioSource.pitch = 1f;
                audioSource.Stop();
                audioSource.PlayOneShot(pierceSound);

                if (!stabbedCollider.transform.GetComponent<JointCollision>())
                {
                    Physics.IgnoreCollision(stabbedCollider, knifeCollider);
                }
                else
                {
                    foreach (Collider collider in stabbedCollider.transform.GetComponent<JointCollision>().collidersOnNPC)
                    {
                        Physics.IgnoreCollision(collider, knifeCollider);
                    }
                }

                joint = gameObject.AddComponent<ConfigurableJoint>();
                joint.axis = new Vector3(0, 90, -1);

                joint.connectedBody = stabbedCollider.transform.GetComponent<Rigidbody>();

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
            }
        }
        if (isPiercing && stabbedCollider)
        {
            float distanceHit = Vector3.Distance(pierce.position, hitPoint.position);

            if (distanceHit < 0.015)
            {
                if (!stabbedCollider.transform.GetComponent<JointCollision>())
                {
                    Physics.IgnoreCollision(stabbedCollider, knifeCollider, false);
                }
                else 
                {
                    foreach (Collider collider in stabbedCollider.transform.GetComponent<JointCollision>().collidersOnNPC)
                    {
                        Physics.IgnoreCollision(collider, knifeCollider, false);
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
        yield return new WaitForSeconds(0.25f);
        isPiercing = false;

        knifeCollider.enabled = true;
        Destroy(hitObject);
    }
}
