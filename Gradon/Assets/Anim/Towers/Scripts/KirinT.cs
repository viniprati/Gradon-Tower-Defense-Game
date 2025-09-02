// KirinT.cs (CORRIGIDO)
using UnityEngine;
using System.Collections.Generic;

public class KirinT : TowerWithBuffs
{
    // --- CORREÇÃO AQUI ---
    // Adicionamos a variável que estava faltando para definir o raio de buff inicial.
    [Header("Atributos do Kirin (Nível 1)")]
    [Tooltip("O raio inicial da aura de buff no Nível 1.")]
    [SerializeField] private float initialAttackRange = 4.0f;

    [Header("Atributos do Buff")]
    [Tooltip("O quanto a velocidade de ataque das torres próximas será multiplicada.")]
    [SerializeField] private float attackRateMultiplier = 1.5f;
    [Tooltip("O quanto o dano das torres próximas será multiplicado.")]
    [SerializeField] private float damageMultiplier = 1.2f;
    [Tooltip("Por quantos segundos o buff dura depois que uma torre sai do alcance.")]
    [SerializeField] private float buffDuration = 2.0f;
    [SerializeField] private float checkInterval = 1.0f;

    private List<TowerWithBuffs> buffedTowers = new List<TowerWithBuffs>();

    protected override void Start()
    {
        // Define os valores para a classe base.
        baseDamage = 0;
        baseAttackRate = 0;
        // Agora a variável 'initialAttackRange' existe e pode ser usada aqui.
        baseAttackRange = initialAttackRange;

        base.Start();

        InvokeRepeating("ApplyBuffToNearbyTowers", 0f, checkInterval);
    }

    private void ApplyBuffToNearbyTowers()
    {
        buffedTowers.RemoveAll(tower => tower == null);
        List<TowerWithBuffs> towersCurrentlyBuffed = new List<TowerWithBuffs>(buffedTowers);

        foreach (TowerWithBuffs tower in towersCurrentlyBuffed)
        {
            if (Vector2.Distance(transform.position, tower.transform.position) > attackRange)
            {
                tower.RemoveBuff();
                buffedTowers.Remove(tower);
            }
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Towers"));

        foreach (var col in colliders)
        {
            TowerWithBuffs tower = col.GetComponent<TowerWithBuffs>();
            if (tower != null && tower != this && !buffedTowers.Contains(tower))
            {
                tower.ApplyBuff(damageMultiplier, attackRateMultiplier, checkInterval + 1f);
                buffedTowers.Add(tower);
            }
        }
    }

    void OnDestroy()
    {
        foreach (var tower in buffedTowers)
        {
            if (tower != null)
            {
                tower.RemoveBuff();
            }
        }
    }

    protected override void Attack() { }
    protected override void HandleDamageBuff(float multiplier, bool isApplying) { }
}