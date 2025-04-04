using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    public bool colliding;
    private void Start()
    {
        StartCoroutine(Loop());
    }
    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.layer == 0 || collision.gameObject.layer == 12)
        {
            colliding = true;
        }
    }
    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            colliding = false;
        }
    }
}
