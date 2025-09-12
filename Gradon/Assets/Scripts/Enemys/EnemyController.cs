// Enemy.cs (CORREÇÃO: Removida a lógica de desaceleração por acerto de projétil)
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Atributos Base")]
    [SerializeField] protected int health;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackRange = 1f; // Alcance de ataque padrão
    [SerializeField] protected float decelerationDistance = 3f; // Distância para começar a desacelerar
    // REMOVIDO: [SerializeField] protected float hitSlowFactor = 0.5f; // Fator de desaceleração ao ser atingido

    protected Rigidbody2D rb;
    protected Transform target; // O alvo do inimigo (jogador/totem)

    public bool IsDead { get; protected set; }
    public event Action<Enemy> OnDeath;

    // REMOVIDO: protected bool isHitByProjectile = false; // Flag para controle de desaceleração por acerto

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        IsDead = false;
        if (Totem.instance != null)
        {
            target = Totem.instance.transform;
        }
        else
        {
            Debug.LogWarning("Totem.instance não encontrado. O inimigo não terá um alvo para seguir.");
        }
    }

    // REMOVIDO: public void ApplyHitEffect() { ... } // Método para o projétil chamar

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
            Vector3 direction = (target.position - transform.position).normalized;
            float currentSpeed = speed;

            // Aplica desaceleração se estiver dentro da decelerationDistance
            if (distanceToTarget < decelerationDistance)
            {
                float decelerationFactor = Mathf.InverseLerp(attackRange, decelerationDistance, distanceToTarget);
                currentSpeed = speed * decelerationFactor;
            }

            // REMOVIDO: if (isHitByProjectile) { currentSpeed *= hitSlowFactor; }

            rb.linearVelocity = direction * currentSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            Attack();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0 && !IsDead)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        IsDead = true;
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    public abstract void Attack();
}