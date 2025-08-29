// Enemy.cs 

using UnityEngine;
using UnityEngine.UI;

// --- MUDANÇA 1: O nome da classe agora é 'Enemy' para corresponder ao nome do arquivo
// e ao que o projétil está procurando.
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Atributos Base")]
    [SerializeField] protected int maxHealth = 100; // Ajustado para int
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected int scoreValue = 10;

    [Header("Referências")]
    [SerializeField] protected Image healthBarFill;

    protected Transform currentTarget;

    // --- Variáveis Internas ---
    protected Rigidbody2D rb;
    protected int currentHealth; // Ajustado para int
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

        // A lógica de busca de alvo permanece a mesma
        if (Totem.instance != null)
        {
            currentTarget = Totem.instance.transform;
        }
        else
        {
            Totem totem = FindFirstObjectByType<Totem>();
            if (totem != null)
            {
                currentTarget = totem.transform;
            }
            else
            {
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
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        moveDirection = (currentTarget.position - transform.position).normalized;
        FlipTowardsTarget();
    }

    private void FixedUpdate()
    {
        if (isDead || currentTarget == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 movementVector = HandleMovement();
        Vector2 separationVector = avoidStacking ? CalculateSeparationVector() : Vector2.zero;
        Vector2 finalVelocity = (movementVector + (separationVector * separationForce)).normalized * speed;

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

    // --- MUDANÇA 2: O método agora aceita um 'int' para o dano,
    // que é o tipo de dado que o projétil envia por padrão.
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        UpdateHealthBar();
        if (currentHealth <= 0) Die();
    }

    // Implementação da interface IDamageable que seu código antigo usava.
    // Ele simplesmente chama o método principal.
    public void TakeDamage(float damage)
    {
        TakeDamage(Mathf.RoundToInt(damage));
    }

    protected void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            // Precisamos converter para float para a divisão funcionar corretamente
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero;

        if (Totem.instance != null)
        {
            Totem.instance.AddMana(scoreValue);
        }

        Destroy(gameObject, 0.1f);
    }
}