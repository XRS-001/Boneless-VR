using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbingPhysics : MonoBehaviour
{
    public Collider[] handColliders;
    public Transform thresholdTransform;
    public float thresholdDistance = 0.15f;
    public InputActionProperty grabInputSource;
    public HandPresencePhysics handPresence;
    private Collider climbingSurface;
    private Transform originalTarget;
    private FixedJoint fixedJoint;
    public bool isGrabbing = false;
    public bool isValidGrab = false;
    private bool isGrabButtonPressed;
    private void Start()
    {
        originalTarget = handPresence.target;
    }
    void FixedUpdate()
    {
        isGrabButtonPressed = grabInputSource.action.ReadValue<float>() > 0.1;
        if (isGrabButtonPressed && !isGrabbing)
        {
            if (climbingSurface != null && isValidGrab && climbingSurface.CompareTag("ClimbingSurface") && !isGrabbing)
            {
                isGrabbing = true;
                Debug.Log(isGrabButtonPressed);
                fixedJoint = gameObject.AddComponent<FixedJoint>();
                fixedJoint.autoConfigureConnectedAnchor = false;
                fixedJoint.connectedAnchor = transform.position;
                handPresence.target = transform;
            }
        }
        else if ((!isGrabButtonPressed || Vector3.Distance(handPresence.transform.position, thresholdTransform.position) > thresholdDistance) && isGrabbing)
        {
            isGrabbing = false;
            isValidGrab = false;
            if (fixedJoint)
            {
                foreach (Collider collider in handColliders)
                {
                    collider.gameObject.SetActive(false);
                }
                StartCoroutine(Delay());
                Destroy(fixedJoint);
                handPresence.target = originalTarget;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        isValidGrab = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isValidGrab = false;   
    }
    private void OnCollisionEnter(Collision collision)
    {
        climbingSurface = collision.collider;
    }
    private void OnCollisionExit(Collision collision)
    {
        climbingSurface = null;
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);

        foreach(Collider collider in handColliders)
        {
            collider.gameObject.SetActive(true);
        }
    }
}