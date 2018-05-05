using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class Zombie_Network : NetworkBehaviour
{
    public GameObject target;

    public float health;
    public float damage;
    public float attackRadius;
    public float movementSpeed;
    public float rotationSpeed;
    public float findTargetInterval;
    public float despawnInterval;
    public float attackInterval;

    private Animator animator;
    private NavMeshAgent agent;
    private float findTargetTimer;
    private float attackTimer;
    private float despawnTimer;
    private bool isAlive;

    public void TakeDamage(float damage)
    {
        if (!isServer)
        {
            return;
        }

        health -= damage;

        if (health <= 0)
        {
            isAlive = false;
        }
    }


    [ServerCallback]
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Idle");
        agent = GetComponent<NavMeshAgent>();
        findTargetTimer = 0;
        attackTimer = 0;
        despawnTimer = 0;
        isAlive = true;
    }

    [ServerCallback]
    void Update()
    {
        if (isAlive)
        {
            AcquireTarget();

            if (target != null)
            {
                FaceLocation(target.transform.position);

                if (Vector3.Distance(target.transform.position, transform.position) > attackRadius)
                {
                    Move();
                }
                else
                {
                    Attack();
                }
            }
            else
            {
                animator.SetTrigger("Idle");
            }
        }
        else
        {
            Death();
        }
    }

    void AcquireTarget()
    {
        findTargetTimer = UpdateTimer(findTargetTimer, findTargetInterval);
        if (findTargetTimer == 0)
        {
            target = FindClosestPlayer();
        }
    }

    GameObject FindClosestPlayer()
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Vector3 direction = player.transform.position - transform.position;
            float distance = direction.sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = player;
            }
        }
        return closestTarget;
    }

    void Move()
    {
        agent.isStopped = false;
        agent.speed = movementSpeed;
        agent.SetDestination(target.transform.position);
        animator.SetTrigger("Move");
    }

    void Attack()
    {
        agent.isStopped = true;
        attackTimer = UpdateTimer(attackTimer, attackInterval);
        if (attackTimer == 0)
        {
            animator.SetTrigger("Attack");
            Debug.Log("Attack");
            // if collision do damage
        }
        else
        {
            animator.SetTrigger("Idle");
        }
    }

    void Death()
    {
        animator.SetTrigger("FallBack");
        agent.isStopped = true;
        //gameObject.GetComponent<Rigidbody>().isKinematic = false;
        despawnTimer = UpdateTimer(despawnTimer, despawnInterval);
        if (despawnTimer == 0)
        {
            Destroy(gameObject);
        }
    }

    void FaceLocation(Vector3 target)
    {
        Vector3 lookDirection = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(lookDirection.x, 0.0f, lookDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    float UpdateTimer(float timer, float duration)
    {
        float tempTimer = timer;
        tempTimer += Time.deltaTime;

        if (tempTimer >= duration)
        {
            return 0;
        }
        return tempTimer;
    }
}
