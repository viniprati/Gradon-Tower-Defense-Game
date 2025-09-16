
using UnityEngine;

public class RangedEnemy : Enemy
{
    // A vari�vel 'attackRate' foi MOVIDA para a classe base 'Enemy.cs'
    // e pode ser configurada no Inspector do prefab.

    [Header("Atributos de Ataque � Dist�ncia")]
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    private float attackCooldown = 0f;

    // Sobrescrevemos o m�todo Update para controlar o cooldown, j� que
    // a classe base j� decide quando chamar PerformAttack.
    protected override void Update()
    {
        base.Update(); // Executa a l�gica de movimento e decis�o da classe base

        // Controla o tempo de recarga do ataque
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    // Sobrescreve o m�todo de ataque da classe base com a l�gica de atirar
    protected override void PerformAttack()
    {
        // S� ataca se o cooldown tiver acabado
        if (attackCooldown <= 0f)
        {
            // L�gica para criar e atirar um proj�til
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