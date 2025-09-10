// Enemy.cs

using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyController : MonoBehaviour
{
    [Header("Atributos Base")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected int manaOnKill = 10;

    [Header("Referências")]
    [SerializeField] protected Image healthBarFill;

    [Header("Comportamento de Separação")]
    [SerializeField] private bool avoidStacking = true;
    [SerializeField] private float separationForce = 5f;
    [SerializeField] private float separationRadius = 1f;

    public event System.Action<Enemy> OnDeath;

    protected Transform currentTarget;
    protected Rigidbody2D rb;
    protected int currentHealth;
    protected bool isDead = false;

    private Vector2 moveDirection;
    private bool isFacingRight = true;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (Totem.instance != null) { currentTarget = Totem.instance.transform; }
        else { Debug.LogError("Nenhuma base (Totem) encontrada na cena!", this); }

        UpdateHealthBar();
    }

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

        Vector2 movementVector = (currentTarget.position - transform.position).normalized;
        Vector2 separationVector = avoidStacking ? CalculateSeparationVector() : Vector2.zero;
        Vector2 finalVelocity = (movementVector + (separationVector * separationForce)).normalized * speed;
        rb.velocity = finalVelocity;
    }

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

    public void TakeDamage(float damage)
    {
        TakeDamage(Mathf.RoundToInt(damage));
    }

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

        if (Totem.instance != null)
        {
            Totem.instance.AddMana(manaOnKill);
        }

        OnDeath?.Invoke(this);

        Destroy(gameObject, 0.1f);
    }
}