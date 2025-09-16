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

    protected Transform currentTarget;
    protected Rigidbody2D rb;
    protected int currentHealth;
    protected bool isDead = false;

    public event System.Action<Enemy> OnDeath;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        if (Totem.instance != null) { currentTarget = Totem.instance.transform; }
    }

    protected virtual void Update()
    {
        if (isDead || currentTarget == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    // --- CORREÇÃO 2 ---
    // Método principal para receber dano como 'int', cumprindo o novo contrato.
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    // --- CORREÇÃO 1 ---
    // Método público para que outros scripts possam verificar se o inimigo está morto.
    public bool IsDead()
    {
        return isDead;
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke(this);

        if (Totem.instance != null)
        {
            Totem.instance.AddMana(manaOnKill);
        }

        Destroy(gameObject);
    }
}