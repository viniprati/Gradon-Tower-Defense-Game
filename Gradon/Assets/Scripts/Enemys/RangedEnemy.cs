// RangedEnemy.cs
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Atributos de Ataque à Distância")]
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    private float attackCooldown = 0f;

    /// <summary>
    /// Sobrescrevemos o FixedUpdate para mudar o comportamento de movimento.
    /// </summary>
    protected override void FixedUpdate()
    {
        if (isDead || target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // Se estiver fora do alcance, continua se movendo (chama a lógica da classe base)
        if (distanceToTarget > attackRange)
        {
            base.FixedUpdate();
        }
        // Se estiver dentro do alcance, PARA e se prepara para atirar
        else
        {
            rb.velocity = Vector2.zero; // Para de se mover!
        }
    }

    // Usamos o Update normal para controlar o timer de ataque
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
        // Lógica para criar e atirar um projétil
        GameObject projGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile proj = projGO.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Seek(target, damage); // Supondo que seu projétil tenha este método
        }
    }
}