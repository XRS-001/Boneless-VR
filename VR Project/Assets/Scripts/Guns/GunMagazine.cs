using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMagazine : MonoBehaviour
{
    public string magazineName;
    public int ammoCapacity;
    public GameObject bulletChild;
    public bool empty;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ammoCapacity == 0)
        {
            Destroy(bulletChild);
        }
    }
}
