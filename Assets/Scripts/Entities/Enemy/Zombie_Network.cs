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
    public float despawnCastTime;
    public float attackCooldown;
    public float attackCastTime;

    private float findTargetTimer;
    private float attackCooldownTimer;
    private float attackCastTimeTimer;
    private float despawnTimer;

    private NavMeshAgent agent;
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
        attackCastTimeTimer = 0;
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

            if (target == null || !target.GetComponent<Health>().isAlive)
            {
                Stop();
                return;
            }

            // Face the target
            FaceLocation(target.transform.position);

            // Update attack timer
            if (attackCooldownTimer == 0)
            {
                canAttack = true;
            }
            attackCooldownTimer = UpdateTimer(attackCooldownTimer, attackCooldown);

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
        if (findTargetTimer == 0)
        {
            target = FindClosestPlayer();
        }
        findTargetTimer = UpdateTimer(findTargetTimer, findTargetCooldown);
    }

    GameObject FindClosestPlayer()
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (!player.GetComponent<Player_Network>().GetComponent<Health>().isAlive)
            {
                continue;
            }

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

    void Stop()
    {
        agent.isStopped = false;
        agent.speed = 0;
        animate.ClearAnimator();
    }

    void Walk()
    {
        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.SetDestination(target.transform.position);
        animate.SetAnimatorBool("Walking", true);
    }

    void Run()
    {
        agent.isStopped = false;
        agent.speed = RunSpeed;
        agent.SetDestination(target.transform.position);
        animate.SetAnimatorBool("Running", true);
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
            // if collision do damage
            target.GetComponent<Player_Network>().TakeDamage(playerControllerId, damage);
        }
    }

    void Death()
    {
        animate.SetAnimatorTrigger("FallBack");
        agent.isStopped = true;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        despawnTimer = UpdateTimer(despawnTimer, despawnCastTime);
        if (despawnTimer == 0)
        {
            Destroy(gameObject);
            GameObject spawner = GameObject.FindGameObjectWithTag("Spawner");
            spawner.GetComponent<Spawner>().zombiesAlive--;
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
