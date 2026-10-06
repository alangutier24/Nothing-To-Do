using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack }
    State current;

    [Header("AI Configuration")]
    [Tooltip("The target (Player) for the enemy to chase.")]
    [SerializeField] private GameObject player;

    [Tooltip("The target (Player) for the enemy to chase.")]
    [SerializeField] private Transform target;

    [Tooltip("Speed at which the enemy moves.")]
    [SerializeField] private float speed = 3.5f;

    [Tooltip("Speed at which the enemy moves.")]
    [SerializeField] private float chaseSpeed = 3.5f;
    
    [Tooltip("How quickly the enemy turns to face the target.")]
    [SerializeField] private float rotationSpeed = 5f;

    [Tooltip("How quickly the enemy turns to face the target.")]
    [SerializeField] protected NavMeshAgent agent;

    [Tooltip("How quickly the enemy turns to face the target.")]
    [SerializeField] float distanceTreshhold = 16f;

    [Tooltip("How quickly the enemy turns to face the target.")]
    public List<Transform> waypoints;

    [SerializeField] float closeRadius = 1f;
    [SerializeField] float closeOffset = 1f;    // cuánto adelanta la esfera
    [SerializeField] LayerMask detectMask;

    public float viewAngle = 30;

    // Components
    private Rigidbody rb;
    private EnemyHealth healthComponent;

    //
    private float chaseUpdate = 0f;
    public float chaseUpdateThreshhold = 1f;
    private float lostPlayer = 0f;
    public float lostPlayerThreshhold = 2f;


    public virtual void EnemyAwake()
    {
        rb = GetComponent<Rigidbody>();
        healthComponent = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
    }

    public virtual void EnemyStart()
    {
        if (target == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("Player not found! Ensure the player is tagged 'Player'.");
                enabled = false;
                return;
            }
        }

        agent.stoppingDistance = 0.5f;
        agent.speed = speed;

        current = State.Patrol;

        GoToRandomWaypoint();
    }

    public virtual void EnemyFixedUpdate()
    {
        if (healthComponent == null && healthComponent.currentHealth <= 0) return;

        switch (current)
        {
            case State.Patrol: PatrolUpdate(); break;
            case State.Chase:  ChaseUpdate();  break;
        }

    }

    public virtual bool HasArrived()
    {
        if (agent.pathPending) return false;
        if (agent.remainingDistance > agent.stoppingDistance) return false;
        return !agent.hasPath || agent.velocity.sqrMagnitude < 0.01f;
    }

    public virtual void ChangeState(State next)
    {
        // Exit del estado actual
        switch (current)
        {
            case State.Patrol: agent.ResetPath(); break;
            case State.Chase:  agent.ResetPath(); break;
        }

        current = next;

        // Enter del nuevo
        switch (current)
        {
            case State.Patrol: GoToRandomWaypoint(); break;
            case State.Chase:  agent.speed = chaseSpeed; break;
        }
    }

    public virtual void ChaseUpdate()
    {

        if (!CanSeePlayer())
        {
            lostPlayer += Time.unscaledDeltaTime;
            if(lostPlayer > lostPlayerThreshhold)
            {
                ChangeState(State.Patrol);
                lostPlayer = 0;
            }
            return;
        }

        chaseUpdate += Time.unscaledDeltaTime;
        if(chaseUpdate < chaseUpdateThreshhold) return;

        chaseUpdate = 0;

        agent.ResetPath();
        agent.SetDestination(player.transform.position);
        
    }

    public virtual void PatrolUpdate()
    {
        if (CanSeePlayer())
        {
            ChangeState(State.Chase);
            return;
        }

        if (agent.pathPending) return;
        if (agent.remainingDistance < 0.5f) GoToRandomWaypoint();
    }

    public virtual void GoToRandomWaypoint()
    {
        if (waypoints.Count == 0)
            return;

        int newWaypointIndex = Random.Range(0, waypoints.Count);

        agent.SetDestination(waypoints[newWaypointIndex].position);
    }

    readonly Collider[] hits = new Collider[8];

    bool CanSeePlayer()
    {
        if(Vector3.Distance(transform.position, player.transform.position) > distanceTreshhold)
            return false;

        Vector3 center = transform.position + transform.forward * closeOffset + Vector3.up;
        int count = Physics.OverlapSphereNonAlloc(center, closeRadius, hits, detectMask);

        Collider found;

        for (int i = 0; i < count; i++)
        {
            if (hits[i].transform.root == transform.root) continue; // ignorarse a sí mismo
            found = hits[i];
            return true;
        }
        found = null;

        Vector3 toPlayer = player.transform.position - transform.position;
        
        Vector3 flatDir = new Vector3(toPlayer.x, 0, toPlayer.z);
        if (Vector3.Angle(transform.forward, flatDir) > viewAngle * 0.5f)
        {
            return false;
        } 

        return true;    
    
    }

}
