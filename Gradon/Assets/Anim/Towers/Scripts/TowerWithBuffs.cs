// TowerWithBuffs.cs (Atualizado com Sistema de Upgrades)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --- NOVA ESTRUTURA DE DADOS PARA UPGRADES ---
// Esta classe pequena serve como um "molde" para cada nível de upgrade.
// [System.Serializable] faz com que ela apareça de forma organizada no Inspector.
[System.Serializable]
public class UpgradeLevel
{
    public int upgradeCost;
    public int newDamage;
    public float newAttackRate;
    public float newAttackRange;
    // Você pode adicionar mais coisas aqui no futuro, como um novo sprite para a torre!
    // public Sprite newSprite;
}

public abstract class TowerWithBuffs : TowerBase
{
    // --- NOVAS VARIÁVEIS PARA O SISTEMA DE UPGRADE ---
    [Header("Configurações de Upgrade")]
    [Tooltip("Configure aqui os diferentes níveis de upgrade para esta torre.")]
    [SerializeField] protected List<UpgradeLevel> upgrades;

    // Nível atual da torre (0 = nível 1, 1 = nível 2, etc.)
    private int currentUpgradeLevel = 0;


    // Nossas variáveis de status base que já existiam
    protected int baseDamage;
    protected float baseAttackRate;

    // O resto do seu script...

    protected override void Start()
    {
        // Chama o Start da classe mãe (TowerBase)
        base.Start();

        // O 'attackRate' e 'attackRange' da TowerBase são inicializados aqui.
        // O 'baseDamage' é definido nas classes filhas (SamuraiTower, DragonT).
        baseAttackRate = attackRate;
    }

    /// <summary>
    /// Chamado pela Unity quando o jogador clica no colisor deste objeto.
    /// </summary>
    private void OnMouseDown()
    {
        // Quando a torre é clicada, tenta fazer o upgrade.
        TryUpgrade();
    }

    /// <summary>
    /// A lógica principal para fazer o upgrade da torre.
    /// </summary>
    public void TryUpgrade()
    {
        // 1. Verifica se já não estamos no nível máximo.
        if (currentUpgradeLevel >= upgrades.Count)
        {
            Debug.Log(gameObject.name + " já está no nível máximo!");
            return;
        }

        // 2. Pega as informações do próximo nível de upgrade.
        UpgradeLevel nextUpgrade = upgrades[currentUpgradeLevel];

        // 3. Verifica se o jogador tem mana suficiente.
        if (Totem.instance != null && Totem.instance.currentMana >= nextUpgrade.upgradeCost)
        {
            // 4. Gasta a mana.
            Totem.instance.SpendMana(nextUpgrade.upgradeCost);

            // 5. Aplica os novos status à torre.
            // Atualizamos as variáveis 'base' para que os buffs sejam calculados corretamente.
            baseDamage = nextUpgrade.newDamage;
            baseAttackRate = nextUpgrade.newAttackRate;

            // Atualiza as variáveis da classe TowerBase diretamente.
            attackRate = nextUpgrade.newAttackRate;
            attackRange = nextUpgrade.newAttackRange;

            // 6. Avança para o próximo nível.
            currentUpgradeLevel++;

            Debug.Log($"<color=cyan>{gameObject.name} atualizado para o Nível {currentUpgradeLevel + 1}!</color>");
            // Adicione um efeito visual de upgrade aqui, se quiser!
        }
        else
        {
            Debug.Log("Mana insuficiente para o upgrade!");
            // Adicione um som de "erro" aqui!
        }
    }

    // Métodos de buff que você já tinha
    public abstract void HandleDamageBuff(float multiplier, bool isApplying);

    // Adicione esta versão de ApplyBuff que seu KirinTower precisa
    public void ApplyBuff(float damageMultiplier, float rateMultiplier, float duration)
    {
        attackRate = baseAttackRate * rateMultiplier;
        HandleDamageBuff(damageMultiplier, true);

        // Você pode adicionar uma coroutine aqui para remover o buff após a duração
    }
}