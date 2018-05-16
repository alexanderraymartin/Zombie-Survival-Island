using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Animate))]
public class Zombie_Network : NetworkBehaviour
{
    public GameObject target;

    public float damage;
    public float attackRadius;
    public float walkSpeed;
    public float RunSpeed;
    public float rotationSpeed;
    public float findTargetCooldown;
    public float despawnWaitTime;
    public float attackCooldown;
    public float attackCastTime;

    private NavMeshAgent agent;
    private float findTargetTimer;
    private float attackCooldownTimer;
    private float attackCastTimeTimer;
    private float despawnTimer;
    private bool canAttack;
    private bool isRunning;
    private Animate animate;
    private Health health;

    [ServerCallback]
    void Start()
    {
        animate = GetComponent<Animate>();
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        findTargetTimer = 0;
        attackCooldownTimer = 0;
        despawnTimer = 0;
        canAttack = true;
        isRunning = false;
    }

    [ServerCallback]
    void Update()
    {
        if (health.isAlive)
        {
            // Find the nearest target
            AcquireTarget();

            if (target == null)
            {
                return;
            }

            // Face the target
            FaceLocation(target.transform.position);

            // Update attack timer
            attackCooldownTimer = UpdateTimer(attackCooldownTimer, attackCooldown);
            if (attackCooldownTimer == 0)
            {
                canAttack = true;
            }

            // Check for distance to target
            if (Vector3.Distance(target.transform.position, transform.position) > attackRadius)
            {
                Move();
            }
            // Check attack cooldown
            else if (canAttack)
            {
                Attack();
            }
        }
        else
        {
            Death();
        }
    }

    void AcquireTarget()
    {
        findTargetTimer = UpdateTimer(findTargetTimer, findTargetCooldown);
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
        if (isRunning)
        {
            Run();
        }
        else
        {
            Walk();
        }
    }

    void Walk()
    {
        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.SetDestination(target.transform.position);
        animate.SetAnimatorTrigger("Walk");
    }

    void Run()
    {
        agent.isStopped = false;
        agent.speed = RunSpeed;
        agent.SetDestination(target.transform.position);
        animate.SetAnimatorTrigger("Run");
    }

    void Attack()
    {
        attackCastTimeTimer = UpdateTimer(attackCastTimeTimer, attackCastTime);
        if (attackCastTimeTimer == 0)
        {
            agent.isStopped = true;
            animate.SetAnimatorTrigger("Attack");
            canAttack = false;
            Debug.Log("Attack");
            // if collision do damages
        }
    }

    void Death()
    {
        animate.SetAnimatorTrigger("FallBack");
        agent.isStopped = true;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        despawnTimer = UpdateTimer(despawnTimer, despawnWaitTime);
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
