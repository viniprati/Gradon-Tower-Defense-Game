using UnityEngine;

public class NormalEnemy : Enemy
{
    [Header("Atributos de Ataque do Normal Enemy")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float attackDamage = 15f;

    private float attackCooldown = 0f;

    protected override void Update()
    {
        if (isDead || currentTarget == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        if (distanceToTarget > attackRange)
        {
            base.Update(); // movimento padrão
        }
        else
        {
            rb.velocity = Vector2.zero;

            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                Attack();
                attackCooldown = 1f / attackRate;
            }
        }
    }

    private void Attack()
    {
        IDamageable targetHealth = currentTarget.GetComponent<IDamageable>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage);
            Debug.Log(gameObject.name + " atacou " + currentTarget.name);
        }
    }
}
