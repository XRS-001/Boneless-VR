using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public float health;
    public PuppetMaster puppet;
    public Animator animator;
    private bool canDamage = true;

    private float speed;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    private bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    private void Awake()
    {
        speed = agent.speed;
        player = GameObject.Find("Camera Offset").transform;
    }
    private void Patroling()
    {
        animator.SetBool("Walking", false);
        agent.speed = speed / 2;
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
            agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = agent.transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(agent.transform.position.x + randomX, agent.transform.position.y, agent.transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -agent.transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    private void ChasePlayer()
    {
        animator.SetBool("Attacking", false);
        animator.SetBool("Walking", true);
        agent.speed = speed;
        agent.SetDestination(player.position);

        agent.transform.LookAt(player.position);
    }
    private void AttackPlayer()
    {
        animator.SetBool("Attacking", true);
        agent.SetDestination(agent.transform.position);

        agent.transform.LookAt(player.position);
        if(!alreadyAttacked)
        {
            int punchAnim = Random.Range(1, 3);
            switch (punchAnim)
            {
                case 1:
                    animator.SetTrigger("Punch");
                    break;
                case 2:
                    animator.SetTrigger("RightJab");
                    break;
            }
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckCapsule(agent.transform.position, new Vector3(agent.transform.position.x, agent.height, agent.transform.position.z), sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckCapsule(agent.transform.position, new Vector3(agent.transform.position.x, agent.height, agent.transform.position.z), attackRange, whatIsPlayer);
        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
        health = Mathf.Clamp(health, 0, 100);
        if(health <= 0)
        {
            puppet.state = PuppetMaster.State.Dead;
        }
    }
    public void DealDamage(float damage, float delayTime)
    {
        if(canDamage)
        {
            health -= damage;
            StartCoroutine(Delay(delayTime));
        }
    }
    IEnumerator Delay(float delayTime)
    {
        canDamage = false;
        yield return new WaitForSeconds(delayTime);
        canDamage = true;
    }
}
