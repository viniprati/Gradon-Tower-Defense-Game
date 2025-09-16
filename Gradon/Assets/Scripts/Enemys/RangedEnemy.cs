
using UnityEngine;

public class RangedEnemy : Enemy
{
    // A variável 'attackRate' foi MOVIDA para a classe base 'Enemy.cs'
    // e pode ser configurada no Inspector do prefab.

    [Header("Atributos de Ataque à Distância")]
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    private float attackCooldown = 0f;

    // Sobrescrevemos o método Update para controlar o cooldown, já que
    // a classe base já decide quando chamar PerformAttack.
    protected override void Update()
    {
        base.Update(); // Executa a lógica de movimento e decisão da classe base

        // Controla o tempo de recarga do ataque
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    // Sobrescreve o método de ataque da classe base com a lógica de atirar
    protected override void PerformAttack()
    {
        // Só ataca se o cooldown tiver acabado
        if (attackCooldown <= 0f)
        {
            // Lógica para criar e atirar um projétil
            GameObject projGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile proj = projGO.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Seek(target, damage);
            }

            // Reseta o cooldown
            attackCooldown = 1f / attackRate;
        }
    }
}