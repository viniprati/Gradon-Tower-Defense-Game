using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Atributos Base")]
    [SerializeField] protected int health;
    [SerializeField] protected float speed;
    protected Rigidbody2D rb;

    public bool IsDead { get; protected set; }

    public event Action<Enemy> OnDeath;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        IsDead = false;
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
