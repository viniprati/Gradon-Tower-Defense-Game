using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Atributos de Ataque à Distância")]
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 5f;

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
            base.Update(); // continua se movendo
        }
        else
        {
            rb.velocity = Vector2.zero;

            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                Shoot();
                attackCooldown = 1f / attackRate;
            }
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || currentTarget == null) return;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D prb = projectile.GetComponent<Rigidbody2D>();
        if (prb != null)
        {
            Vector2 direction = (currentTarget.position - transform.position).normalized;
            prb.velocity = direction * projectileSpeed;
        }

        Debug.Log(gameObject.name + " disparou em " + currentTarget.name);
    }
}
