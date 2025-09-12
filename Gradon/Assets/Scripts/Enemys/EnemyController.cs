// Enemy.cs
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Atributos Base do Inimigo")]
    [SerializeField] protected int health = 100; // Vida padrão
    [SerializeField] protected float speed = 3f; // Velocidade padrão de movimento
    [SerializeField] protected float attackRange = 1.5f; // Distância para parar e atacar
    [SerializeField] protected float decelerationStartDistance = 5f; // Distância para começar a desacelerar
    [SerializeField] protected int collisionDamage = 10; // Dano padrão ao colidir/tocar o Totem

    protected Rigidbody2D rb;
    protected Transform target; // O alvo a ser perseguido (geralmente o Totem)

    public bool IsDead { get; protected set; }
    public event Action<Enemy> OnDeath; // Evento para notificar quando o inimigo morre

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        IsDead = false;

        // Tenta encontrar o Totem como alvo principal
        if (Totem.instance != null)
        {
            target = Totem.instance.transform;
        }
        else
        {
            Debug.LogWarning("Totem.instance não encontrado. O inimigo não terá um alvo para seguir.", this);
        }

        // Ignora colisão com o Totem (apenas para evitar empurrar, o dano é no Attack ou Trigger)
        if (target != null && target.GetComponent<Collider2D>() != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), target.GetComponent<Collider2D>());
        }

        // Ignora colisão com outras Torres
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        foreach (var tower in towers)
        {
            Collider2D towerCol = tower.GetComponent<Collider2D>();
            if (towerCol != null)
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), towerCol);
        }
    }

    protected virtual void Update()
    {
        MoveTowardsTarget();
    }

    /// <summary>
    /// Gerencia o movimento do inimigo em direção ao alvo, incluindo desaceleração.
    /// </summary>
    protected void MoveTowardsTarget()
    {
        if (target == null || IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > attackRange)
        {
            // Calcula a direção normalizada para o alvo
            Vector3 direction = (target.position - transform.position).normalized;
            float currentMoveSpeed = speed;

            // Aplica desaceleração se estiver dentro da 'decelerationStartDistance'
            if (distanceToTarget < decelerationStartDistance)
            {
                // Calcula um fator de desaceleração (1.0 = velocidade total, 0.0 = parado)
                // Quanto mais perto de attackRange, menor o fator.
                float decelerationFactor = Mathf.InverseLerp(attackRange, decelerationStartDistance, distanceToTarget);
                currentMoveSpeed = speed * decelerationFactor;
            }

            // Aplica a velocidade ao Rigidbody2D
            rb.linearVelocity = direction * currentMoveSpeed;
        }
        else
        {
            // Se estiver dentro do range de ataque, para o inimigo
            rb.linearVelocity = Vector2.zero;
            Attack(); // Chama o método de ataque específico de cada inimigo
        }
    }

    /// <summary>
    /// Aplica dano ao inimigo e verifica se ele deve morrer.
    /// </summary>
    /// <param name="damage">A quantidade de dano a ser aplicada.</param>
    public void TakeDamage(int damage)
    {
        if (IsDead) return; // Não aplica dano se já estiver morto

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Lógica de morte do inimigo.
    /// </summary>
    protected virtual void Die()
    {
        if (IsDead) return; // Garante que a lógica de morte só seja executada uma vez

        IsDead = true;
        OnDeath?.Invoke(this); // Notifica outros sistemas que o inimigo morreu
        Destroy(gameObject); // Remove o GameObject da cena
    }

    /// <summary>
    /// Método abstrato para o ataque, a ser implementado por cada inimigo específico.
    /// </summary>
    public abstract void Attack();

    // Opcional: Para desenhar o raio de desaceleração e ataque no Editor
    protected virtual void OnDrawGizmosSelected()
    {
        // Raio de alcance de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Raio de início da desaceleração
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, decelerationStartDistance);
    }
}