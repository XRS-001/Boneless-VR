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
    private Collider stabbedCollider;
    private ConfigurableJoint joint;
    private Vector3 lastPosition;
    public float velocity;
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        velocity = distance / Time.deltaTime;
        lastPosition = transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pierceable") && velocity > requiredSpeedToPierce)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.thisCollider == bladeTipCollider)
                {
                    audioSource.pitch = 1f;
                    audioSource.PlayOneShot(pierceSound);
                    isPiercing = true;

                    stabbedCollider = collision.collider;
                    Physics.IgnoreCollision(stabbedCollider, knifeCollider);
                    Physics.IgnoreCollision(stabbedCollider, bladeTipCollider);
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
        if (collision.gameObject.CompareTag("Pierceable") && isPiercing)
        {
            isPiercing = false;
            audioSource.pitch = 1.5f;
            audioSource.PlayOneShot(pierceSound);
            Destroy(joint);
            stabbedCollider = collision;
            StartCoroutine(Delay());
        }
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);

        Physics.IgnoreCollision(stabbedCollider, knifeCollider, false);
        Physics.IgnoreCollision(stabbedCollider, bladeTipCollider, false);
        isPiercing = false;
    }
}
