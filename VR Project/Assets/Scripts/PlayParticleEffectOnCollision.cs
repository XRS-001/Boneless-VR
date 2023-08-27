using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleEffectOnCollision : MonoBehaviour
{
    public ParticleSystem effect;
    private bool canEffect = true;
    private void OnCollisionEnter(Collision collision)
    {
        if(effect != null && canEffect)
        {
            effect.Play();
        }
        canEffect = false;
    }
}
