// Enemy.cs

using UnityEngine;

// [RequireComponent] garante que qualquer objeto com este script
// terá obrigatoriamente um Rigidbody2D e um Collider2D.
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

    // Variáveis protegidas (acessíveis por esta classe e pelas classes filhas)
    protected Transform target;
    protected Rigidbody2D rb;
    protected float currentHealth;
    protected bool isDead = false;
    private float attackCooldown = 0f;

    // Evento para notificar o spawner sobre a morte
    public event System.Action<Enemy> OnDeath;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Configura o Rigidbody para ser Kinematic, ideal para este tipo de movimento.
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0; // Garante que não haja influência da gravidade.
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        // Encontra o alvo (Totem) no início do jogo.
        if (Totem.instance != null)
        {
            target = Totem.instance.transform;
        }
    }

    protected virtual void Update()
    {
        if (isDead || target == null) return;

        // Gerencia o cooldown do ataque.
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        // Decide se deve mover ou atacar com base na distância para o alvo.
        if (Vector2.Distance(transform.position, target.position) > attackRange)
        {
            Move();
        }
        else
        {
            PerformAttack();
        }
    }

    // ======================================================================
    // MÉTODO CORRIGIDO
    // ======================================================================
    protected void Move()
    {
        // Em vez de mover o 'transform.position', usamos 'rb.MovePosition'.
        // Este é o método correto para mover Rigidbodies do tipo Kinematic,
        // pois interage corretamente com o motor de física e evita que o
        // Rigidbody "durma" e pare de se mover.
        Vector2 newPosition = Vector2.MoveTowards(rb.position, target.position, speed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }
    // ======================================================================

    protected virtual void PerformAttack()
    {
        if (attackCooldown <= 0f)
        {
            if (target != null && Totem.instance != null)
            {
                Totem.instance.TakeDamage(attackDamage);
            }
            // Reseta o cooldown do ataque.
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

        // Notifica o spawner (ou qualquer outro script interessado) que este inimigo morreu.
        OnDeath?.Invoke(this);

        // Dá a recompensa de mana ao jogador.
        if (Totem.instance != null)
        {
            Totem.instance.AddMana(manaOnKill);
        }

        // Remove o inimigo da cena.
        Destroy(gameObject);
    }
}