using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;

public class KnifeSlice : MonoBehaviour
{
    private KnifePierce knifePierce;
    public AudioSource audioSource;
    public AudioClip sliceSound;
    private float velocity;
    public float speedNeededToSlice;
    private bool hasHit;
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public VelocityEstimator velocityEstimator;
    public LayerMask sliceableLayer;
    public Material crossSectionMaterial;
    private void Start()
    {
        knifePierce = GetComponent<KnifePierce>() ?? null;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
        hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        if(knifePierce != null)
        {
            if (hasHit && velocity > speedNeededToSlice && !knifePierce.isPiercing)
            {
                GameObject target = hit.transform.gameObject;
                Slice(target);
            }
        }
        else
        {
            if (hasHit && velocity > speedNeededToSlice)
            {
                GameObject target = hit.transform.gameObject;
                Slice(target);
            }
        }

    }
    public void Slice(GameObject target)
    {
        audioSource.PlayOneShot(sliceSound);
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if(hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            SetupSlicedComponent(upperHull);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            SetupSlicedComponent(lowerHull);

            Destroy(target);
        }
    }
    public void SetupSlicedComponent(GameObject slicedObject)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        rb.mass = 10.0f;
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
    }
}
