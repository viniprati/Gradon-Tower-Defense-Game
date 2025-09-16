// Enemy.cs

using UnityEngine;

// A interface para garantir que qualquer coisa possa receber dano.
public interface IDamageable
{
    void TakeDamage(int damage);
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    public event System.Action<Enemy> OnDeath;

    [Header("Atributos Base")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected int manaOnKill = 10;
    [SerializeField] protected int damageToTotem = 50;

    // Variáveis protegidas para que as classes filhas possam acessá-las
    protected float currentHealth;
    protected Transform target;
    protected Rigidbody2D rb;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        // Encontra o Totem como alvo. Se não encontrar, para de funcionar.
        if (Totem.instance != null)
        {
            target = Totem.instance.transform;
        }
        else
        {
            Debug.LogError("Nenhum Totem encontrado na cena! Inimigos não têm um alvo.", this);
            this.enabled = false;
        }
    }

    /// <summary>
    /// A lógica de movimento agora está em FixedUpdate para consistência com a física.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        if (isDead || target == null) return;

        // Movimento direto e simples, sem forças complexas.
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        // Adicione aqui a lógica da barra de vida (UpdateHealthBar) se tiver

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke(this); // Avisa ao spawner que morreu

        if (Totem.instance != null)
        {
            Totem.instance.AddMana(manaOnKill);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Lógica para quando o inimigo atinge fisicamente o totem.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Usamos OnCollisionEnter para ter certeza de que é um impacto físico.
        if (collision.gameObject.CompareTag("Totem"))
        {
            Totem.instance.TakeDamage(damageToTotem);
            Die(); // O inimigo se sacrifica ao atacar o totem
        }
    }
}