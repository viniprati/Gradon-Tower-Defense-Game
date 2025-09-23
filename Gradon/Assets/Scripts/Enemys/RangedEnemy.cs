// RangedEnemy.cs
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Referências do Projétil")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    // O RangedEnemy tem um comportamento de ataque diferente, então ele sobrescreve este método.
    protected override void PerformAttack()
    {
        // O cooldown já é controlado pelo Update() da classe base,
        // que chama este método na cadência correta.

        // A única coisa que precisamos fazer aqui é atirar.
        if (projectilePrefab != null && firePoint != null && target != null)
        {
            GameObject projGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile proj = projGO.GetComponent<Projectile>();
            if (proj != null)
            {
                // Usa a variável 'attackDamage' herdada da classe base
                proj.Seek(target, this.attackDamage);
            }
        }
    }
}