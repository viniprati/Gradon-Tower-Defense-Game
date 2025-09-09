// TowerWithBuffs.cs (Atualizado com Sistema de Upgrades)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --- NOVA ESTRUTURA DE DADOS PARA UPGRADES ---
// Esta classe pequena serve como um "molde" para cada n�vel de upgrade.
// [System.Serializable] faz com que ela apare�a de forma organizada no Inspector.
[System.Serializable]
public class UpgradeLevel
{
    public int upgradeCost;
    public int newDamage;
    public float newAttackRate;
    public float newAttackRange;
    // Voc� pode adicionar mais coisas aqui no futuro, como um novo sprite para a torre!
    // public Sprite newSprite;
}

public abstract class TowerWithBuffs : TowerBase
{
    // --- NOVAS VARI�VEIS PARA O SISTEMA DE UPGRADE ---
    [Header("Configura��es de Upgrade")]
    [Tooltip("Configure aqui os diferentes n�veis de upgrade para esta torre.")]
    [SerializeField] protected List<UpgradeLevel> upgrades;

    // N�vel atual da torre (0 = n�vel 1, 1 = n�vel 2, etc.)
    private int currentUpgradeLevel = 0;


    // Nossas vari�veis de status base que j� existiam
    protected int baseDamage;
    protected float baseAttackRate;

    // O resto do seu script...

    protected override void Start()
    {
        // Chama o Start da classe m�e (TowerBase)
        base.Start();

        // O 'attackRate' e 'attackRange' da TowerBase s�o inicializados aqui.
        // O 'baseDamage' � definido nas classes filhas (SamuraiTower, DragonT).
        baseAttackRate = attackRate;
    }

    /// <summary>
    /// Chamado pela Unity quando o jogador clica no colisor deste objeto.
    /// </summary>
    private void OnMouseDown()
    {
        // Quando a torre � clicada, tenta fazer o upgrade.
        TryUpgrade();
    }

    /// <summary>
    /// A l�gica principal para fazer o upgrade da torre.
    /// </summary>
    public void TryUpgrade()
    {
        // 1. Verifica se j� n�o estamos no n�vel m�ximo.
        if (currentUpgradeLevel >= upgrades.Count)
        {
            Debug.Log(gameObject.name + " j� est� no n�vel m�ximo!");
            return;
        }

        // 2. Pega as informa��es do pr�ximo n�vel de upgrade.
        UpgradeLevel nextUpgrade = upgrades[currentUpgradeLevel];

        // 3. Verifica se o jogador tem mana suficiente.
        if (Totem.instance != null && Totem.instance.currentMana >= nextUpgrade.upgradeCost)
        {
            // 4. Gasta a mana.
            Totem.instance.SpendMana(nextUpgrade.upgradeCost);

            // 5. Aplica os novos status � torre.
            // Atualizamos as vari�veis 'base' para que os buffs sejam calculados corretamente.
            baseDamage = nextUpgrade.newDamage;
            baseAttackRate = nextUpgrade.newAttackRate;

            // Atualiza as vari�veis da classe TowerBase diretamente.
            attackRate = nextUpgrade.newAttackRate;
            attackRange = nextUpgrade.newAttackRange;

            // 6. Avan�a para o pr�ximo n�vel.
            currentUpgradeLevel++;

            Debug.Log($"<color=cyan>{gameObject.name} atualizado para o N�vel {currentUpgradeLevel + 1}!</color>");
            // Adicione um efeito visual de upgrade aqui, se quiser!
        }
        else
        {
            Debug.Log("Mana insuficiente para o upgrade!");
            // Adicione um som de "erro" aqui!
        }
    }

    // M�todos de buff que voc� j� tinha
    public abstract void HandleDamageBuff(float multiplier, bool isApplying);

    // Adicione esta vers�o de ApplyBuff que seu KirinTower precisa
    public void ApplyBuff(float damageMultiplier, float rateMultiplier, float duration)
    {
        attackRate = baseAttackRate * rateMultiplier;
        HandleDamageBuff(damageMultiplier, true);

        // Voc� pode adicionar uma coroutine aqui para remover o buff ap�s a dura��o
    }
}