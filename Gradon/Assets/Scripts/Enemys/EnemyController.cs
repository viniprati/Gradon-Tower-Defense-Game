using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float damage = 10f;
    public float speed = 3.5f;

    protected NavMeshAgent agent;
    protected Transform target;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        if (target != null)
            agent.SetDestination(target.position);
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
