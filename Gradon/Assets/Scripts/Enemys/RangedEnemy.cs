// RangedEnemy.cs (CORRIGIDO)
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Configurações do Ranged Enemy")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileDamage = 30f;

    private float fireCooldown = 0f;

    protected override void Start()
    {
        base.Start();
        attackRange = 7.0f;
        decelerationStartDistance = 9.0f;
    }

    protected override void Update()
    {
        base.Update();

        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    public override void Attack()
    {
        if (fireCooldown > 0f) return;

        if (projectilePrefab != null && firePoint != null && target != null)
        {
            GameObject projGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = projGO.GetComponent<Projectile>();

            if (projectile != null)
            {
                Vector2 direction = (target.position - firePoint.position).normalized;
                projectile.Launch(direction, projectileSpeed, projectileDamage);
                fireCooldown = fireRate;
            }
        }
    }
}