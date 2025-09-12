// Enemy.cs (CORRE��O: Removida a l�gica de desacelera��o por acerto de proj�til)
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Atributos Base")]
    [SerializeField] protected int health;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackRange = 1f; // Alcance de ataque padr�o
    [SerializeField] protected float decelerationDistance = 3f; // Dist�ncia para come�ar a desacelerar
    // REMOVIDO: [SerializeField] protected float hitSlowFactor = 0.5f; // Fator de desacelera��o ao ser atingido

    protected Rigidbody2D rb;
    protected Transform target; // O alvo do inimigo (jogador/totem)

    public bool IsDead { get; protected set; }
    public event Action<Enemy> OnDeath;

    // REMOVIDO: protected bool isHitByProjectile = false; // Flag para controle de desacelera��o por acerto

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
            Debug.LogWarning("Totem.instance n�o encontrado. O inimigo n�o ter� um alvo para seguir.");
        }
    }

    // REMOVIDO: public void ApplyHitEffect() { ... } // M�todo para o proj�til chamar

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

            // Aplica desacelera��o se estiver dentro da decelerationDistance
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