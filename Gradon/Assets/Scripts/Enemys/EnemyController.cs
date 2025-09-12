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

    protected Rigidbody2D rb;
    protected Transform target;
    public bool IsDead { get; protected set; }
    public event Action<Enemy> OnDeath;

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
            rb.linearVelocity = direction * speed;
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
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (IsDead) return;
        IsDead = true;
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    public abstract void Attack();

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}