// Enemy.cs

using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Atributos Base")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected int manaOnKill = 10;
    [SerializeField] protected int damageToTotem = 50;

    [Header("Comportamento de Ataque")]
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float attackRate = 1f;

    // Vari�veis protegidas para que as classes filhas possam acess�-las
    protected Transform target;
    protected Rigidbody2D rb;
    protected float currentHealth;
    protected bool isDead = false;

    public event System.Action<Enemy> OnDeath;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Usa a forma moderna e correta de definir um Rigidbody Kinematic
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        if (Totem.instance != null) { target = Totem.instance.transform; }
    }

    /// <summary>
    /// Update controla a l�gica de decis�o do inimigo.
    /// Foi marcado como 'virtual' para permitir que classes filhas (como RangedEnemy) o modifiquem.
    /// </summary>
    protected virtual void Update()
    {
        if (isDead || target == null) return;

        if (Vector2.Distance(transform.position, target.position) > attackRange)
        {
            Move();
        }
        else
        {
            PerformAttack();
        }
    }

    /// <summary>
    /// Move o inimigo em dire��o ao alvo.
    /// </summary>
    protected void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    /// <summary>
    /// O m�todo de ataque padr�o (contato).
    /// Foi marcado como 'virtual' para permitir que RangedEnemy o substitua por um ataque � dist�ncia.
    /// </summary>
    protected virtual void PerformAttack()
    {
        // Esta � a l�gica para inimigos de contato.
        if (Totem.instance != null)
        {
            Totem.instance.TakeDamage(damageToTotem);
        }
        Die(); // O inimigo se sacrifica no ataque
    }

    /// <summary>
    /// Implementa o m�todo da interface IDamageable.
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    /// <summary>
    /// L�gica de morte do inimigo.
    /// </summary>
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke(this);
        if (Totem.instance != null) { Totem.instance.AddMana(manaOnKill); }
        Destroy(gameObject);
    }
}