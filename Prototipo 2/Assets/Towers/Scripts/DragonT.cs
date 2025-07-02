// Dentro do script RangedTower.cs
using UnityEngine;

public class RangedTower : MonoBehaviour
{
    [Header("Atributos da Torre")]
    public float attackRange = 5f;
    public float attackRate = 1f;
    public int damage = 5;

    [Header("Referências")]
    public GameObject projectilePrefab; // Arraste o prefab do projétil aqui

    private Transform currentTarget;
    private float attackCooldown = 0f;

    void Update()
    {
        FindTarget();

        attackCooldown -= Time.deltaTime;
        if (currentTarget != null && attackCooldown <= 0f)
        {
            Shoot();
            attackCooldown = 1f / attackRate;
        }
    }

    void FindTarget()
    {
        // Encontra o inimigo MAIS PRÓXIMO dentro do raio
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (var col in collidersInRange)
        {
            if (col.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector2.Distance(transform.position, col.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = col.transform;
                }
            }
        }

        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            currentTarget = nearestEnemy;
        }
        else
        {
            currentTarget = null;
        }
    }

    void Shoot()
    {
        GameObject projGO = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectil projectile = projGO.GetComponent<Projectil>();

        if (projectile != null)
        {
            projectile.target = currentTarget;
            projectile.damage = this.damage; // Passa o dano da torre para o projétil
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}