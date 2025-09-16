// RangedEnemy.cs
using UnityEngine;

public class RangedEnemy : Enemy // Herda da classe base correta
{
    [Header("Ataque à Distância")]
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    private float attackCooldown = 0f;

    // Sobrescrevemos o FixedUpdate para parar de se mover quando está no alcance
    protected override void FixedUpdate()
    {
        if (isDead || target == null) return;

        if (Vector2.Distance(transform.position, target.position) > attackRange)
        {
            base.FixedUpdate(); // Chama o movimento da classe base
        }
        else
        {
            rb.velocity = Vector2.zero; // Para de se mover
        }
    }

    // Usamos Update para o timer de ataque, que não envolve física
    private void Update()
    {
        if (isDead || target == null) return;

        attackCooldown -= Time.deltaTime;

        if (Vector2.Distance(transform.position, target.position) <= attackRange)
        {
            if (attackCooldown <= 0f)
            {
                Attack();
                attackCooldown = 1f / attackRate;
            }
        }
    }

    private void Attack()
    {
        GameObject projGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile proj = projGO.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Seek(target, damage);
        }
    }
}