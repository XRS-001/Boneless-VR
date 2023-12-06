using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffectOnShot : MonoBehaviour
{
    public GameObject bulletWound;
    public GameObject bulletHole;
    public ParticleSystem effect;
    public ParticleSystem bloodEffect;
    public BloodDecal decal;
    public GameObject trail;
    private bool canEffect = true;
    private void Update()
    {
        if (effect != null)
        {
            trail.SetActive(true);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer != 20 && effect != null && canEffect)
        {
            effect.transform.parent = null;
            effect.Play();
            GetComponent<Rigidbody>().isKinematic = true;
            GameObject spawnedBulletHole = Instantiate(bulletHole);
            spawnedBulletHole.transform.position = collision.GetContact(0).point;
            spawnedBulletHole.transform.rotation = Quaternion.LookRotation(collision.GetContact(0).normal);
            spawnedBulletHole.transform.parent = collision.gameObject.transform;

        }
        if(collision.gameObject.layer == 20 && canEffect && effect != null)
        {
            bloodEffect.transform.parent = null;
            bloodEffect.Play();
            decal.Decal(transform.position);
            GameObject spawnedBulletHole = Instantiate(bulletWound);
            spawnedBulletHole.transform.position = collision.GetContact(0).point;
            spawnedBulletHole.transform.rotation = Quaternion.LookRotation(collision.GetContact(0).normal);
            spawnedBulletHole.transform.parent = collision.gameObject.transform;
        }
        canEffect = false;
    }
}
