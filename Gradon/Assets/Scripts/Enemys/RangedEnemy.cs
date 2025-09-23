// RangedEnemy.cs
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Refer�ncias do Proj�til")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    // O RangedEnemy tem um comportamento de ataque diferente, ent�o ele sobrescreve este m�todo.
    protected override void PerformAttack()
    {
        // O cooldown j� � controlado pelo Update() da classe base,
        // que chama este m�todo na cad�ncia correta.

        // A �nica coisa que precisamos fazer aqui � atirar.
        if (projectilePrefab != null && firePoint != null && target != null)
        {
            GameObject projGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile proj = projGO.GetComponent<Projectile>();
            if (proj != null)
            {
                // Usa a vari�vel 'attackDamage' herdada da classe base
                proj.Seek(target, this.attackDamage);
            }
        }
    }
}