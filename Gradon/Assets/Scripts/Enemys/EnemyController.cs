// Enemy.cs (Refeito com MoveTowards)

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

    // --- NOVA VARIÁVEL ---
    [Header("Comportamento de Ataque")]
    [Tooltip("A distância que o inimigo vai parar do totem para atacar.")]
    [SerializeField] protected float attackRange = 1.5f;

    // Variáveis protegidas
    protected Transform target;
    protected Rigidbody2D rb;
    protected float currentHealth;
    protected bool isDead = false;

    public event System.Action<Enemy> OnDeath;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Garante que a física não interfira no nosso movimento com MoveTowards
        rb.isKinematic = true;

        currentHealth = maxHealth;
        if (Totem.instance != null) { target = Totem.instance.transform; }
    }

    /// <summary>
    /// Update agora controla tanto a lógica de movimento quanto a de ataque.
    /// </summary>
    protected virtual void Update()
    {
        if (isDead || target == null) return;

        // Calcula a distância atual até o totem
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // Se estivermos FORA do alcance de ataque, nos movemos.
        if (distanceToTarget > attackRange)
        {
            MoveTowardsTarget();
        }
        // Se estivermos DENTRO do alcance, paramos e tentamos atacar.
        else
        {
            // A lógica de ataque específica (corpo a corpo, à distância)
            // será implementada nas classes filhas (NormalEnemy, RangedEnemy, etc.).
            PerformAttack();
        }
    }

    /// <summary>
    /// Método que move o inimigo em direção ao alvo usando MoveTowards.
    /// </summary>
    protected void MoveTowardsTarget()
    {
        // Calcula a posição do próximo passo
        Vector2 newPosition = Vector2.MoveTowards(
            transform.position,      // Posição atual
            target.position,         // Posição do alvo
            speed * Time.deltaTime   // Distância a ser movida neste frame
        );

        // Aplica a nova posição ao transform do inimigo
        transform.position = newPosition;
    }

    /// <summary>
    /// As classes filhas vão sobrescrever este método para definir seu tipo de ataque.
    /// </summary>
    protected virtual void PerformAttack()
    {
        // O inimigo básico (NormalEnemy) pode ter seu ataque de contato aqui.
        // O RangedEnemy terá sua própria lógica.
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    public bool IsDead()
    {
        return isDead;
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