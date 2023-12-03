using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BloodDecal : MonoBehaviour
{
    public GameObject decal;
    public void Decal(Vector3 position)
    {
        LayerMask layerMask = 1 << 12;
        Physics.Raycast(position, Vector3.down, out RaycastHit hit, 12, layerMask);
        Instantiate(decal);
        decal.transform.position = hit.point;
        Debug.Log(hit.transform.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
