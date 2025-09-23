// Enemy.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Atributos Base")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected int manaOnKill = 10;

    [Header("Comportamento de Ataque")]
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float attackRate = 1f; // Ataques por segundo
    [SerializeField] protected int attackDamage = 10;

    protected Transform target;
    protected Rigidbody2D rb;
    protected float currentHealth;
    protected bool isDead = false;
    private float attackCooldown = 0f;

    public event System.Action<Enemy> OnDeath;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        if (Totem.instance != null) { target = Totem.instance.transform; }
    }

    protected virtual void Update()
    {
        if (isDead || target == null) return;

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (Vector2.Distance(transform.position, target.position) > attackRange)
        {
            Move();
        }
        else
        {
            PerformAttack();
        }
    }

    protected void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    protected virtual void PerformAttack()
    {
        if (attackCooldown <= 0f)
        {
            if (target != null && Totem.instance != null)
            {
                Totem.instance.TakeDamage(attackDamage);
            }
            attackCooldown = 1f / attackRate;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke(this);
        if (Totem.instance != null) { Totem.instance.AddMana(manaOnKill); }
        Destroy(gameObject);
    }
}