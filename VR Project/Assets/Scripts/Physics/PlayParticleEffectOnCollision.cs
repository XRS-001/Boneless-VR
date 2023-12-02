using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleEffectOnCollision : MonoBehaviour
{
    public ParticleSystem effect;
    public ParticleSystem bloodEffect;
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
        }
        if(collision.gameObject.layer == 20 && canEffect && effect != null)
        {
            bloodEffect.transform.parent = null;
            bloodEffect.Play();
        }
        canEffect = false;
    }
}
