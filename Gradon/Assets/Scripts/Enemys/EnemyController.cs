// Enemy.cs 
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Atributos Base")]
    [SerializeField] protected float health = 100f;
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float decelerationStartDistance = 5f;

    protected Rigidbody2D rb;
    protected Transform target;
    public bool IsDead { get; protected set; }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        IsDead = false;
        if (Totem.instance != null)
        {
            target = Totem.instance.transform;
        }
    }

    protected virtual void Update()
    {
        if (IsDead) return;
        MoveTowardsTarget();
    }

    protected void MoveTowardsTarget()
    {
        if (target == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > attackRange)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float currentSpeed = speed;

            if (distanceToTarget < decelerationStartDistance)
            {
                float factor = Mathf.InverseLerp(attackRange, decelerationStartDistance, distanceToTarget);
                currentSpeed = speed * factor;
            }
            rb.linearVelocity = direction * currentSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            Attack();
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        health -= damage;
        Debug.Log($"{gameObject.name} tomou {damage} de dano. Vida restante: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (IsDead) return;
        IsDead = true;
        Debug.Log($"<color=red>{gameObject.name} morreu!</color>");
        Destroy(gameObject);
    }

    public abstract void Attack();
}