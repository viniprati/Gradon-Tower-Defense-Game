// RangedEnemy.cs (Adaptado para o novo Projectile.cs)
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
        // Aumenta o alcance para que ele pare antes e atire
        attackRange = 7.0f;
        decelerationStartDistance = 9.0f;
    }

    protected override void Update()
    {
        base.Update();

        // O cooldown só deve ser contado quando o inimigo está parado e pronto para atacar
        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    public override void Attack()
    {
        if (fireCooldown > 0f) return; // Se ainda estiver em cooldown, não ataca

        if (projectilePrefab != null && firePoint != null && target != null)
        {
            Debug.Log($"{gameObject.name} atacando o Totem!");

            GameObject projGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = projGO.GetComponent<Projectile>();

            // Calcula a direção para o alvo
            Vector2 direction = (target.position - firePoint.position).normalized;

            // Lança o projétil
            projectile.Launch(direction, projectileSpeed, projectileDamage);

            // Reseta o cooldown
            fireCooldown = fireRate;
        }
    }
}