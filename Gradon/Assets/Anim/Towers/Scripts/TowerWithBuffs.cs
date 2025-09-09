// TowerWithBuffs.cs (Atualizado para se conectar com a UI)

using System.Collections;
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
    [Header("Configura��es de Upgrade")]
    [SerializeField] protected List<UpgradeLevel> upgrades;

    private int currentUpgradeLevel = 0;

    protected int baseDamage;
    protected float baseAttackRate;

    protected override void Start()
    {
        base.Start();
        baseAttackRate = attackRate;
        // Garante que o indicador de alcance comece escondido
        ShowRangeIndicator(false);
    }

    /// <summary>
    /// Chamado quando a torre � clicada. Agora, ele abre o painel da UI.
    /// </summary>
    private void OnMouseDown()
    {
        if (TowerInfoPanel.instance != null)
        {
            TowerInfoPanel.instance.ShowPanel(this);
        }
    }

    /// <summary>
    /// A l�gica de upgrade permanece a mesma, mas agora � chamada pelo BOT�O da UI.
    /// </summary>
    public void TryUpgrade()
    {
        if (IsAtMaxLevel())
        {
            Debug.Log(gameObject.name + " j� est� no n�vel m�ximo!");
            return;
        }

        UpgradeLevel nextUpgrade = upgrades[currentUpgradeLevel];

        // Vamos assumir que Totem.SpendMana retorna 'true' se a compra foi bem sucedida
        if (Totem.instance != null && Totem.instance.SpendMana(nextUpgrade.upgradeCost))
        {
            baseDamage = nextUpgrade.newDamage;
            baseAttackRate = nextUpgrade.newAttackRate;
            attackRate = nextUpgrade.newAttackRate;
            attackRange = nextUpgrade.newAttackRange;

            currentUpgradeLevel++;

            Debug.Log($"<color=cyan>{gameObject.name} atualizado para o N�vel {currentUpgradeLevel + 1}!</color>");
        }
        else
        {
            Debug.Log("Mana insuficiente para o upgrade!");
        }
    }

    // --- NOVOS M�TODOS P�BLICOS PARA A UI ACESSAR ---

    public bool IsAtMaxLevel()
    {
        return currentUpgradeLevel >= upgrades.Count;
    }

    public int GetNextUpgradeCost()
    {
        if (!IsAtMaxLevel())
        {
            return upgrades[currentUpgradeLevel].upgradeCost;
        }
        return 0; // Retorna 0 se j� estiver no n�vel m�ximo
    }

    // --- FIM DOS NOVOS M�TODOS ---

    public abstract void HandleDamageBuff(float multiplier, bool isApplying);

    public void ApplyBuff(float damageMultiplier, float rateMultiplier, float duration)
    {
        attackRate = baseAttackRate * rateMultiplier;
        HandleDamageBuff(damageMultiplier, true);
        // ... (l�gica de coroutine para remover o buff)
    }

    // AVISO: Certifique-se de que seu Totem.cs tem o m�todo SpendMana retornando um bool!
    // Exemplo:
    /*
        public bool SpendMana(int amount)
        {
            if (currentMana >= amount)
            {
                currentMana -= amount;
                UpdateManaBar();
                return true; // Compra bem sucedida
            }
            return false; // Falha na compra
        }
    */
}