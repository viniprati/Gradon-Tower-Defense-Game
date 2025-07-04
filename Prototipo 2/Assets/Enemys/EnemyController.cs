// EnemyController.cs (Com a lógica de desaparecimento corrigida)
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
    private bool isDead = false; // NOVA: Variável para garantir que a morte só aconteça uma vez

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        PlayerController player = FindFirstObjectByType<PlayerController>(); // Corrigido para o método mais novo
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
        // Se o inimigo já está morto, não faz mais nada.
        if (isDead) return;

        HandleDecision();
    }

    void FixedUpdate()
    {
        // Se o inimigo já está morto, não se move.
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
                // CORREÇÃO: a propriedade correta é 'velocity'
                rb.linearVelocity *= 0.9f;
                return;
            }
        }

        if (hasTarget)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 movementForce = direction * speed * 10f;
            rb.AddForce(movementForce);

            // CORREÇÃO: a propriedade correta é 'velocity'
            if (rb.linearVelocity.magnitude > speed)
            {
                // CORREÇÃO: a propriedade correta é 'velocity'
                rb.linearVelocity = rb.linearVelocity.normalized * speed;
            }
        }
    }

    // --- FUNÇÃO TakeDamage MODIFICADA ---
    public void TakeDamage(float damage)
    {
        // Se o inimigo já está morto, ignora qualquer dano adicional.
        if (isDead) return;

        currentHealth -= damage;

        // Se a vida acabou E ele ainda não foi marcado como morto...
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ReachedEnd()
    {
        // Se o inimigo já está morto, ele não pode chegar ao fim.
        if (isDead) return;

        Debug.Log("Inimigo chegou à base!");
        Destroy(gameObject);
    }

    // --- FUNÇÃO Die MODIFICADA ---
    private void Die()
    {
        // Marca o inimigo como morto para que esta função não seja chamada novamente.
        isDead = true;

        Debug.Log(gameObject.name + " foi derrotado!");

        // Dá a mana ao jogador.
        PlayerController player = FindFirstObjectByType<PlayerController>(); // Corrigido para o método mais novo
        if (player != null && player.gameObject.activeInHierarchy)
        {
            player.AddMana(manaOnDeath);
        }

        // AQUI: É o lugar perfeito para adicionar um efeito de morte, como partículas ou som.
        // Exemplo: Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        // Finalmente, destrói o objeto do inimigo, fazendo-o desaparecer da cena.
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}