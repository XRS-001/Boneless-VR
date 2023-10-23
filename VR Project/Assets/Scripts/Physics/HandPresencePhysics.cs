using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPresencePhysics : MonoBehaviour
{
    public GameObject handColliderParent;
    public Transform target;
    public Collider[] handColliders;
    public ControllerInteractors controller;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Collider collider in handColliders)
        {
            collider.enabled = false;
        }
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (transform.position.y > 0.2)
        {
            foreach (Collider collider in handColliders)
            {
                collider.enabled = true;
            }
        }
        else
        {
            foreach (Collider collider in handColliders)
            {
                collider.enabled = false;
            }
        }
        rb.MovePosition(target.position);
        rb.MoveRotation(target.rotation);
    }
}




