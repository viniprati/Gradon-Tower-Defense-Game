// EnemyBase.cs (Adaptado para Tower Defense)
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Atributos Base")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected int scoreValue = 10; // Ou manaValue, se preferir

    [Header("Referências")]
    [SerializeField] protected Image healthBarFill;

    // --- MUDANÇA: Alvo agora é um Transform genérico (pode ser a base ou uma torre) ---
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

        // --- MUDANÇA: O alvo inicial é sempre a base/totem ---
        TotemHealth totem = FindFirstObjectByType<TotemHealth>();
        if (totem != null)
        {
            currentTarget = totem.transform;
        }
        else
        {
            Debug.LogError("Nenhuma base (Totem) encontrada na cena! Inimigos não têm um alvo.", this);
        }

        UpdateHealthBar();
    }

    // O Update agora determina a direção e vira o sprite em direção ao alvo atual.
    protected virtual void Update()
    {
        if (isDead || currentTarget == null)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        // --- MUDANÇA: Direção agora é baseada no 'currentTarget' ---
        moveDirection = (currentTarget.position - transform.position).normalized;
        FlipTowardsTarget();
    }

    // --- MUDANÇA: Corrigido para 'velocity' e usa 'currentTarget' ---
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

        // CORREÇÃO: A propriedade correta é 'velocity'.
        rb.linearVelocity = finalVelocity;
    }

    protected abstract Vector2 HandleMovement();

    private Vector2 CalculateSeparationVector()
    {
        // ... (seu código de separação está perfeito, não precisa de alterações)
    }

    // --- MUDANÇA: Renomeado e adaptado para o alvo genérico ---
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

    // --- MUDANÇA: Recompensa agora é Mana para o PlayerController ---
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero;

        // Procura pelo PlayerController para dar a recompensa de mana
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            // Assumindo que 'scoreValue' agora representa a mana dropada
            player.AddMana(scoreValue);
        }

        Destroy(gameObject, 0.1f);
    }
}