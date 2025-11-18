// Enemy.cs (Com Diagnóstico de Ataque Adicionado)
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

    // --- FUNÇÃO MODIFICADA COM O "DEDO DURO" ---
    protected virtual void PerformAttack()
    {
        // Dedo duro 1: Confirma que o inimigo está no modo de ataque
        Debug.Log($"[DEDO DURO] {gameObject.name} está no alcance e tentando atacar {target.name}.");

        if (attackCooldown <= 0f)
        {
            // Tenta encontrar um componente de vida no alvo para atacar
            var targetHealth = target.GetComponent<TowerHealth>();

            if (targetHealth != null)
            {
                // Dedo duro 2: Confirma que o ataque vai acontecer
                Debug.Log($"<color=green>[DEDO DURO - SUCESSO]</color> {gameObject.name} encontrou o script TowerHealth e vai causar {attackDamage} de dano.");
                targetHealth.TakeDamage(attackDamage);
            }
            else
            {
                // Dedo duro 3: Grita o erro exato se o ataque falhar
                Debug.LogError($"<color=red>[DEDO DURO - ERRO CRÍTICO]</color> O ataque de {gameObject.name} FALHOU! O alvo '{target.name}' NÃO TEM o script 'TowerHealth.cs' anexado!");
            }

            attackCooldown = 1f / attackRate;
        }
    }
    // ---------------------------------------------

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