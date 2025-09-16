// TowerWithBuffs.cs (Corrigido com atualização do dano no upgrade)

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeLevel
{
    public int upgradeCost;
    public int newDamage;
    public float newAttackRate;
    public float newAttackRange;
}

public abstract class TowerWithBuffs : TowerBase
{
    [Header("Configurações de Upgrade")]
    [SerializeField] protected List<UpgradeLevel> upgrades;

    public int baseDamage { get; protected set; }
    public float baseAttackRange; // Esta variável parece não ser usada, considere remover
    protected float baseAttackRate;
    private int currentUpgradeLevel = 0;

    protected override void Start()
    {
        base.Start();
        baseAttackRate = attackRate;
    }

    private void OnMouseDown()
    {
        if (TowerInfoPanel.instance != null)
        {
            TowerInfoPanel.instance.ShowPanel(this);
        }
    }

    public void TryUpgrade()
    {
        if (IsAtMaxLevel()) return;

        UpgradeLevel nextUpgrade = upgrades[currentUpgradeLevel];

        if (Totem.instance != null && Totem.instance.SpendMana(nextUpgrade.upgradeCost))
        {
            // Aplica os novos status base
            baseDamage = nextUpgrade.newDamage;
            baseAttackRate = nextUpgrade.newAttackRate;
            attackRate = nextUpgrade.newAttackRate;
            attackRange = nextUpgrade.newAttackRange;

            // --- CORREÇÃO ADICIONADA AQUI ---
            // Após o upgrade, chamamos HandleDamageBuff para garantir que o 'damage'
            // da torre filha (como DragonT) seja atualizado para o novo 'baseDamage'.
            // O multiplicador '1' e 'isApplying = false' apenas resetam o dano para o valor base.
            HandleDamageBuff(1, false);

            currentUpgradeLevel++;
            Debug.Log($"{towerName} atualizada para o Nível {currentUpgradeLevel + 1}!");
        }
    }

    public bool IsAtMaxLevel()
    {
        return currentUpgradeLevel >= upgrades.Count;
    }

    public int GetNextUpgradeCost()
    {
        return IsAtMaxLevel() ? 0 : upgrades[currentUpgradeLevel].upgradeCost;
    }

    protected abstract void HandleDamageBuff(float multiplier, bool isApplying);

    public void ApplyBuff(float damageMultiplier, float rateMultiplier, float duration)
    {
        attackRate = baseAttackRate * rateMultiplier;
        HandleDamageBuff(damageMultiplier, true);
    }
}