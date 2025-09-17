// Enemy.cs

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Atributos Base")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected int manaOnKill = 15;
    [SerializeField] protected float attackRate = 1f;

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

        // A vida é inicializada uma única vez aqui. Está correto.
        currentHealth = maxHealth;

        if (Totem.instance != null) { target = Totem.instance.transform; }
    }

    protected virtual void Update()
    {
        if (isDead || target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget > attackRange)
        {
            MoveTowardsTarget();
        }
        else
        {
            PerformAttack();
        }
    }

    protected void MoveTowardsTarget()
    {
        Vector2 newPosition = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
        transform.position = newPosition;
    }

    protected virtual void PerformAttack()
    {
        // Lógica a ser implementada pelas classes filhas
    }

    // --- NOVA FUNÇÃO DE DEBUG ADICIONADA AQUI ---
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // SENSOR 1: Confirma que o método foi chamado e com qual valor.
        Debug.Log($"'{gameObject.name}' recebeu o comando TakeDamage com {damage} de dano.");

        currentHealth -= damage;

        // SENSOR 2: Mostra o estado da vida após a subtração.
        Debug.Log($"Vida de '{gameObject.name}' agora é: {currentHealth} / {maxHealth}");

        // Adicione aqui a lógica da sua barra de vida, se tiver
        // UpdateHealthBar();

        if (currentHealth <= 0)
        {
            // SENSOR 3: Confirma que a condição de morte foi atingida.
            Debug.Log($"<color=red>'{gameObject.name}' atingiu 0 ou menos de vida! Chamando Die().</color>");
            Die();
        }
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