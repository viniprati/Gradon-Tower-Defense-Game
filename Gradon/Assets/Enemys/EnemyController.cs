// EnemyController.cs (Sua classe base, agora completa e corrigida)
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Atributos Base")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected int scoreValue = 10; // Renomeado para manaValue seria mais claro

    [Header("Referências")]
    [SerializeField] protected Image healthBarFill;

    protected Transform currentTarget;

    // --- Variáveis Internas ---
    protected Rigidbody2D rb;
    protected float currentHealth;
    protected bool isDead = false;
    protected Vector2 moveDirection;
    private bool isFacingRight = true;

    [Header("Comportamento de Separação")]
    [SerializeField] private bool avoidStacking = true;
    [SerializeField] private float separationForce = 5f;
    [SerializeField] private float separationRadius = 1f;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // --- LÓGICA DE BUSCA DE ALVO MODIFICADA ---
        // Primeiro, tenta encontrar o alvo usando o Singleton do Totem, que é mais rápido.
        if (Totem.instance != null)
        {
            currentTarget = Totem.instance.transform;
        }
        else
        {
            // Se o Singleton falhar (por exemplo, se o Totem ainda não executou o Awake),
            // tenta uma busca mais lenta na cena como um plano B.
            Totem totem = FindFirstObjectByType<Totem>();
            if (totem != null)
            {
                currentTarget = totem.transform;
            }
            else
            {
                // Se ambas as buscas falharem, aí sim mostra o erro.
                Debug.LogError("Nenhuma base (Totem) encontrada na cena! Inimigos não têm um alvo.", this);
            }
        }

        UpdateHealthBar();
    }

    public bool IsDead()
    {
        return isDead;
    }

    protected virtual void Update()
    {
        if (isDead || currentTarget == null)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero; // Corrigido para 'velocity'
            return;
        }

        moveDirection = (currentTarget.position - transform.position).normalized;
        FlipTowardsTarget();
    }

    private void FixedUpdate()
    {
        if (isDead || currentTarget == null)
        {
            rb.linearVelocity = Vector2.zero; // Corrigido para 'velocity'
            return;
        }

        Vector2 movementVector = HandleMovement();
        Vector2 separationVector = avoidStacking ? CalculateSeparationVector() : Vector2.zero;
        Vector2 finalVelocity = (movementVector + (separationVector * separationForce)).normalized * speed;

        // CORREÇÃO: A propriedade correta é 'velocity'.
        rb.linearVelocity = finalVelocity;
    }

    protected abstract Vector2 HandleMovement();

    private Vector2 CalculateSeparationVector()
    {
        Vector2 steer = Vector2.zero;
        int neighborsCount = 0;
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, separationRadius);

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == this.gameObject || !neighbor.CompareTag("Enemy")) continue;

            Vector2 difference = transform.position - neighbor.transform.position;
            steer += difference.normalized / (difference.magnitude + 0.01f);
            neighborsCount++;
        }

        if (neighborsCount > 0)
        {
            steer /= neighborsCount;
        }
        return steer;
    }

    protected void FlipTowardsTarget()
    {
        if (moveDirection.x > 0 && !isFacingRight) Flip();
        else if (moveDirection.x < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        UpdateHealthBar();
        if (currentHealth <= 0) Die();
    }

    protected void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero; // Corrigido para 'velocity'

        if (Totem.instance != null)
        {
            Totem.instance.AddMana(scoreValue);
        }

        Destroy(gameObject, 0.1f);
    }
}