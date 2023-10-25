using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    public bool colliding;
    private void OnCollisionEnter(Collision collision)
    {
        colliding = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        colliding = false;
    }
}
