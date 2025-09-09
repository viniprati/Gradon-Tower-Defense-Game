// TowerWithBuffs.cs (CORRIGIDO)
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

    // --- MUDANÇA PRINCIPAL: TORNADO 'public' PARA A UI LER ---
    public int baseDamage { get; protected set; } // A UI pode ler, mas só a torre pode mudar
    public float baseAttackRange;
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

        // Supondo que seu Totem.SpendMana retorna um bool
        if (Totem.instance != null && Totem.instance.SpendMana(nextUpgrade.upgradeCost))
        {
            baseDamage = nextUpgrade.newDamage;
            baseAttackRate = nextUpgrade.newAttackRate;
            attackRate = nextUpgrade.newAttackRate;
            attackRange = nextUpgrade.newAttackRange;
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