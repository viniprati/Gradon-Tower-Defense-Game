// RangedTower.cs
using UnityEngine;

public class RangedTower : TowerWithBuffs
{
    [Header("Atributos Ranged")]
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint; // Ponto de onde o projétil sai
    protected override void Start()
    {
        base.Start();
        originalDamage = damage; // Guarda o dano original
    }

    protected override void HandleDamageBuff(float multiplier, bool isApplying)
    {
        damage = isApplying ? Mathf.RoundToInt(originalDamage * multiplier) : originalDamage;
    }
    protected override void Attack()
    {
        if (projectilePrefab == null || firePoint == null || currentTarget == null) return;

        Vector3 spawnPosition = firePoint.position;
        Vector2 direction = currentTarget.position - spawnPosition;
        GameObject projGO = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        Projectile projectileScript = projGO.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            // CHAMADA ATUALIZADA: Passa a direção E o dano atual da torre.
            // A variável 'damage' aqui já pode ter sido alterada pelo sistema de buff.
            projectileScript.Launch(direction, this.damage);
        }
    }
}