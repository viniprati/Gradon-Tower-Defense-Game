// SamuraiTower.cs (Com Sistema de Upgrades Integrado)
using UnityEngine;

public class SamuraiTower : TowerWithBuffs
{
    [Header("Atributos Samurai (Nível 1)")]
    [SerializeField] private int initialDamage = 15;
    [SerializeField] private float initialAttackRate = 1.0f;
    [SerializeField] private float initialAttackRange = 2.0f;
    [SerializeField] private GameObject attackEffectPrefab;

    // A variável 'damage' agora representa o dano ATUAL (com buffs)
    private int damage;

    // O Start agora define os valores BASE para o Nível 1
    protected override void Start()
    {
        // Define os status BASE para o nível inicial
        baseDamage = initialDamage;
        baseAttackRate = initialAttackRate;
        baseAttackRange = initialAttackRange;

        // Chama o Start da classe base, que irá aplicar esses status
        base.Start();
    }

    // O HandleDamageBuff agora trabalha com 'baseDamage'
    protected override void HandleDamageBuff(float multiplier, bool isApplying)
    {
        damage = isApplying ? Mathf.RoundToInt(baseDamage * multiplier) : baseDamage;
    }

    protected override void Attack()
    {
        if (attackEffectPrefab != null)
        {
            Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
        }

        // Usa 'attackRange' que é atualizado pelos upgrades e buffs
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (var col in colliders)
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Usa 'damage' que é atualizado pelos upgrades e buffs
                enemy.TakeDamage(damage);
            }
        }
    }
}