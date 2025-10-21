// Enemy.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour
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
    protected float attackCooldown = 0f;

    public event System.Action<Enemy> OnDeath;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        // Inimigos normais ainda mirarão no totem por padrão
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

    // --- MUDANÇA 1: Adicionada a palavra "virtual" ---
    // Agora o Boss pode ter seu próprio método de ataque
    protected virtual void PerformAttack()
    {
        if (attackCooldown <= 0f)
        {
            // Tenta encontrar um componente de vida no alvo para atacar
            var targetHealth = target.GetComponent<TowerHealth>(); // Supondo que suas torres tenham esse script
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
            }
            // A lógica do Totem.instance foi removida para ser mais genérica

            attackCooldown = 1f / attackRate;
        }
    }

    // --- MUDANÇA 2: Adicionada a palavra "virtual" ---
    // Agora o Boss pode adicionar sua própria lógica ao tomar dano
    public virtual void TakeDamage(float damage)
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