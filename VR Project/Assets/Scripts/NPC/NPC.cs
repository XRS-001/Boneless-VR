using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public float health;
    public PuppetMaster puppet;
    private bool canDamage = true;
    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, 100);
        if(health <= 0)
        {
            puppet.state = PuppetMaster.State.Dead;
        }
    }
    public void DealDamage(float damage)
    {
        if(canDamage)
        {
            health -= damage;
            StartCoroutine(Delay());
        }
    }
    IEnumerator Delay()
    {
        canDamage = false;
        yield return new WaitForSeconds(0.5f);
        canDamage = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");
    }
}
