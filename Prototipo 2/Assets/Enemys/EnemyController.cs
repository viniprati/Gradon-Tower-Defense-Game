// EnemyController.cs (Com a correção de 'velocity')
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Atributos do Inimigo")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float manaOnDeath = 10f;

    [Header("Comportamento de Ataque")]
    [SerializeField] private float detectionRange = 7f;

    // --- Variáveis Internas ---
    private Rigidbody2D rb;
    private float currentHealth;
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private Transform playerTransform;
    private bool isChasingPlayer = false;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Jogador não encontrado na cena! O inimigo não saberá quem perseguir.");
        }

        GameObject pathGO = GameObject.Find("Path");
        if (pathGO != null)
        {
            waypoints = new Transform[pathGO.transform.childCount];
            for (int i = 0; i < pathGO.transform.childCount; i++)
            {
                waypoints[i] = pathGO.transform.GetChild(i);
            }
        }
    }

    void Update()
    {
        if (isDead) return;
        HandleDecision();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        Move();
    }

    private void HandleDecision()
    {
        if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy)
        {
            isChasingPlayer = false;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        isChasingPlayer = (distanceToPlayer <= detectionRange);
    }

    private void Move()
    {
        Vector2 targetPosition;
        bool hasTarget = false;

        if (isChasingPlayer && playerTransform != null)
        {
            targetPosition = playerTransform.position;
            hasTarget = true;
        }
        else
        {
            if (waypoints != null && currentWaypointIndex < waypoints.Length)
            {
                targetPosition = waypoints[currentWaypointIndex].position;
                hasTarget = true;
                if (Vector2.Distance(transform.position, targetPosition) < 0.2f)
                {
                    currentWaypointIndex++;
                }
            }
            else
            {
                if (waypoints != null && currentWaypointIndex >= waypoints.Length)
                {
                    ReachedEnd();
                }

                // CORREÇÃO: Usar 'velocity'
                rb.linearVelocity *= 0.9f;
                return;
            }
        }

        if (hasTarget)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 movementForce = direction * speed * 10f;
            rb.AddForce(movementForce);

            // CORREÇÃO: Usar 'velocity'
            if (rb.linearVelocity.magnitude > speed)
            {
                // CORREÇÃO: Usar 'velocity'
                rb.linearVelocity = rb.linearVelocity.normalized * speed;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ReachedEnd()
    {
        if (isDead) return;
        Debug.Log("Inimigo chegou à base!");
        Destroy(gameObject);
    }

    private void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " foi derrotado!");

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null && player.gameObject.activeInHierarchy)
        {
            player.AddMana(manaOnDeath);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}