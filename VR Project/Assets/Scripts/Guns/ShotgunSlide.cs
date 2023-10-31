using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class ShotgunSlide : MonoBehaviour
{
    private float threshold = 0.02f;
    public Transform target;
    public Transform targetEnd;
    public UnityEvent onReached;
    public bool wasReached = false;
    public bool wasReachedEnd = false;
    public float distance;
    public float distanceEnd;
    private void FixedUpdate()
    {
        distance = Vector3.Distance(transform.position, target.position);

        if (distance < threshold && !wasReached)
        {
            onReached.Invoke();
            wasReached = true;
        }
        else if (distance >= threshold)
        {
            wasReached = false;
        }
        distanceEnd = Vector3.Distance(transform.position, targetEnd.position);

        if (distanceEnd < threshold && !wasReachedEnd)
        {
            wasReachedEnd = true;
        }
        else if (distanceEnd >= threshold)
        {
            wasReachedEnd = false;
        }
    }
}
