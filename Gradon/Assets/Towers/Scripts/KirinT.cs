// KirinT.cs (Buff Tower)
using UnityEngine;
using System.Collections.Generic;

public class KirinT : TowerBase
{
    [Header("Atributos de Buff")]
    [Tooltip("Multiplicador de dano (1.5 = +50% de dano)")]
    [SerializeField] private float damageMultiplier = 1.5f;
    [Tooltip("Multiplicador da taxa de ataque (1.5 = 50% mais rápido)")]
    [SerializeField] private float rateMultiplier = 1.5f;
    [Tooltip("Por quanto tempo o buff dura após ser aplicado")]
    [SerializeField] private float buffDuration = 3f;

    // Kirin não ataca, então vamos sobrescrever o Update para não procurar inimigos
    protected override void Update()
    {
        // A lógica de ataque/cooldown é usada para aplicar o buff periodicamente
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            Attack(); // O "Ataque" aqui é aplicar o buff
            attackCooldown = 1f / attackRate;
        }
    }

    // O ataque da Kirin é dar buff nas torres ao redor
    protected override void Attack()
    {
        // Encontra todas as torres no raio de alcance (buffRange)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (var col in colliders)
        {
            // Tenta pegar o script de outra torre (que não seja esta)
            TowerWithBuffs tower = col.GetComponent<TowerWithBuffs>();
            if (tower != null && tower != this)
            {
                // Aplica o buff na torre encontrada
                tower.ApplyBuff(damageMultiplier, rateMultiplier, buffDuration);
            }
        }
    }
}