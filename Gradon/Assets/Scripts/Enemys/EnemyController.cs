// Enemy.cs 

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    // --- MUDANÇA #1: ORGANIZAÇÃO E RENOMEAÇÃO ---
    [Header("Atributos Base")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected float speed = 2f;
    [Tooltip("Quantidade de mana que o jogador ganha ao derrotar este inimigo.")]
    [SerializeField] protected int manaOnKill = 10; // Renomeado de 'scoreValue' para mais clareza

    [Header("Referências")]
    [SerializeField] protected Image healthBarFill;
    // ... o resto do seu código ...

    // O resto do seu script está perfeito e não precisa de NENHUMA outra mudança.
    // Apenas colei ele aqui para garantir que está completo.
    protected Transform currentTarget;
    protected Rigidbody2D rb;
    protected int currentHealth;
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
        if (Totem.instance != null) { currentTarget = Totem.instance.transform; }
        else
        {
            Totem totem = FindFirstObjectByType<Totem>();
            if (totem != null) { currentTarget = totem.transform; }
            else { Debug.LogError("Nenhuma base (Totem) encontrada na cena!", this); }
        }
        UpdateHealthBar();
    }

    public bool IsDead() { return isDead; }

    protected virtual void Update()
    {
        if (isDead || currentTarget == null)
        {
            if (rb != null) rb.velocity = Vector2.zero;
            return;
        }
        moveDirection = (currentTarget.position - transform.position).normalized;
        FlipTowardsTarget();
    }

    private void FixedUpdate()
    {
        if (isDead || currentTarget == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        Vector2 movementVector = HandleMovement();
        Vector2 separationVector = avoidStacking ? CalculateSeparationVector() : Vector2.zero;
        Vector2 finalVelocity = (movementVector + (separationVector * separationForce)).normalized * speed;
        rb.velocity = finalVelocity;
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
        if (neighborsCount > 0) { steer /= neighborsCount; }
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

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        UpdateHealthBar();
        if (currentHealth <= 0) Die();
    }

    public void TakeDamage(float damage) { TakeDamage(Mathf.RoundToInt(damage)); }

    protected void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;

        // --- MUDANÇA #2: USANDO A VARIÁVEL RENOMEADA ---
        if (Totem.instance != null)
        {
            // Adiciona a quantidade de mana definida em 'manaOnKill'
            Totem.instance.AddMana(manaOnKill);
        }

        // Você está faltando o evento OnDeath aqui, vou adicioná-lo
        // para compatibilidade com o WaveSpawner.
        OnDeath?.Invoke(this);

        Destroy(gameObject, 0.1f);
    }

    // É uma boa prática adicionar este evento para o WaveSpawner saber quando um inimigo morreu.
    public event System.Action<Enemy> OnDeath;
}