using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum AIState { Patrol, Chase, Attack }

    [Header("States")]
    [SerializeField] private AIState currentState = AIState.Patrol;

    [Header("Detect Settings")]
    [SerializeField] private float detectionRadius = 25f;
    [SerializeField] private float attackRadius = 4.5f;
    [SerializeField] private float contactRadius = 4.2f;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waitTimeAtPoint = 2f;
    private int currentPatrolIndex;
    private bool isWaitingAtPoint;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float contactDamageCooldown = 0.8f;
    [SerializeField] private float attackDamage = 20f;
    private bool isAttacking = false;
    private float lastContactDamageTime = -10f;

    private NavMeshAgent agent;
    private Transform playerTransform;
    private HealthSystem healthSystemPlayer;
    private bool isStunned;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            healthSystemPlayer = playerObj.GetComponent<HealthSystem>();
        }
        else
        {
            Debug.LogError("No hay GameObjects con tag Player");
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            GoToNextPatrolPoint();
        }
    }

    private void Update()
    {
                if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
                healthSystemPlayer = playerObj.GetComponent<HealthSystem>() ?? playerObj.GetComponentInChildren<HealthSystem>();
            }
        }

        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        if (isStunned || playerTransform == null || (healthSystemPlayer != null && healthSystemPlayer.IsDead))
        {
            agent.isStopped = true;
            return;
        }

        Vector3 diff = playerTransform.position - transform.position;
        float distanceToPlayer = Mathf.Sqrt(diff.x * diff.x + diff.z * diff.z);
        float verticalDist = Mathf.Abs(diff.y);

        // DAÑO POR CONTACTO: Si el enemigo toca físicamente al jugador (por distancia)
        if (distanceToPlayer <= contactRadius && verticalDist <= 5.5f)
        {
            ApplyContactDamage(playerTransform.gameObject);
        }

        switch (currentState)
        {
            case AIState.Patrol:
                UpdatePatrolState(distanceToPlayer);
                break;

            case AIState.Chase:
                UpdateChaseState(distanceToPlayer);
                break;

            case AIState.Attack:
                UpdateAttackState(distanceToPlayer);
                break;
        }
    }

    private void UpdatePatrolState(float distanceToPlayer)
    {
        agent.isStopped = false;

        if (distanceToPlayer <= detectionRadius)
        {
            currentState = AIState.Chase;
            return;
        }

        if (!agent.pathPending && agent.isOnNavMesh && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaitingAtPoint)
            {
                StartCoroutine(WaitAndMoveToNextPoint());
            }
        }
    }

    private void UpdateChaseState(float distanceToPlayer)
    {
        agent.isStopped = false;
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(playerTransform.position);
        }

        if (distanceToPlayer <= attackRadius)
        {
            currentState = AIState.Attack;
            return;
        }

            if (distanceToPlayer > detectionRadius * 1.2f)
        {
            currentState = AIState.Patrol;
            GoToNextPatrolPoint();
        }
    }

    private void UpdateAttackState(float distanceToPlayer)
    {
        transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));

        
        if (distanceToPlayer > attackRadius * 1.3f && !isAttacking)
        {
            currentState = AIState.Chase;
            return;
        }

        if (!isAttacking)
        {
            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        isAttacking = true;
        if (agent.isOnNavMesh) agent.isStopped = true;

        ExecuteAttack();
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        if (agent.isOnNavMesh) agent.isStopped = false;
    }

    private void ExecuteAttack()
    {
        if (healthSystemPlayer == null && playerTransform != null)
        {
            healthSystemPlayer = playerTransform.GetComponent<HealthSystem>() ?? playerTransform.GetComponentInChildren<HealthSystem>();
        }

        if (healthSystemPlayer != null && !healthSystemPlayer.IsDead)
        {
            lastContactDamageTime = Time.time;
            healthSystemPlayer.TakeDamage(attackDamage);
            Debug.Log($"¡El enemigo atacó al jugador causándole {attackDamage} de daño!");
        }
    }

    public void ApplyContactDamage(GameObject target)
    {
        if (isStunned) return;
        if (Time.time - lastContactDamageTime < contactDamageCooldown) return;

        if (healthSystemPlayer == null && target != null)
        {
            healthSystemPlayer = target.GetComponent<HealthSystem>() ?? target.GetComponentInChildren<HealthSystem>() ?? target.GetComponentInParent<HealthSystem>();
        }

        if (healthSystemPlayer != null && !healthSystemPlayer.IsDead)
        {
            lastContactDamageTime = Time.time;
            healthSystemPlayer.TakeDamage(attackDamage);
            Debug.Log($"<color=red>¡El enemigo tocó al jugador y le infligió {attackDamage} de daño!</color>");

            if (currentState == AIState.Patrol)
            {
                currentState = AIState.Attack;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other.gameObject)) ApplyContactDamage(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsPlayer(other.gameObject)) ApplyContactDamage(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsPlayer(collision.gameObject)) ApplyContactDamage(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (IsPlayer(collision.gameObject)) ApplyContactDamage(collision.gameObject);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (IsPlayer(hit.gameObject)) ApplyContactDamage(hit.gameObject);
    }

    private bool IsPlayer(GameObject obj)
    {
        if (obj == null) return false;
        return obj.CompareTag("Player") || (obj.transform.root != null && obj.transform.root.CompareTag("Player"));
    }

    public void OnAttackedByPlayer()
    {
        currentState = AIState.Chase;
    }

    private IEnumerator WaitAndMoveToNextPoint()
    {
        isWaitingAtPoint = true;
        yield return new WaitForSeconds(waitTimeAtPoint);

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            GoToNextPatrolPoint();
        }

        isWaitingAtPoint = false;
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (agent.isOnNavMesh && patrolPoints[currentPatrolIndex] != null)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    public void Stun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        if (agent.isOnNavMesh) agent.isStopped = true;
        Debug.Log($"Aturdido por {duration} segundos");

        yield return new WaitForSeconds(duration);

        isStunned = false;
        if (agent.isOnNavMesh) agent.isStopped = false;
        Debug.Log("Recuperado del aturdimiento");
    }
}