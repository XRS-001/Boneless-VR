using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbingPhysics : MonoBehaviour
{
    public InputActionProperty grabInputSource;
    private Collider climbingSurface;
    public float radius = 0.1f;
    public LayerMask grabLayer;
    private FixedJoint fixedJoint;
    private bool isGrabbing = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool isGrabButtonPressed = grabInputSource.action.ReadValue<float>() > 0.1;
        if (isGrabButtonPressed && !isGrabbing)
        {
            if (climbingSurface != null)
            {
                Debug.Log(isGrabButtonPressed);
                fixedJoint = gameObject.AddComponent<FixedJoint>();
                fixedJoint.autoConfigureConnectedAnchor = false;

                fixedJoint.connectedAnchor = transform.position;
            }
            isGrabbing = true;
        }
        else if (!isGrabButtonPressed && isGrabbing)
        {
            isGrabbing = false;
            if (fixedJoint)
            {
                Destroy(fixedJoint);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        climbingSurface = collision.collider;
    }
    private void OnCollisionExit(Collision collision)
    {
        climbingSurface = null;
    }
}