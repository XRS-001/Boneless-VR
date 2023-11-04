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
    private bool isAboveGround = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Collider collider in handColliders)
        {
            collider.enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (transform.position.y > 0.2 && !isAboveGround)
        {
            foreach (Collider collider in handColliders)
            {
                collider.enabled = true;
            }
            isAboveGround = true;
        }
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}




