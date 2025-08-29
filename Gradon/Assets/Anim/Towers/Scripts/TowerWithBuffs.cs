// TowerWithBuffs.cs (Com Sistema de Upgrades Integrado)
using System.Collections;
using System.Collections.Generic; // Para usar List<>
using UnityEngine;

// Definição da classe de dados para os níveis de upgrade (coloque no mesmo arquivo)
[System.Serializable]
public class TowerLevelData
{
    public int upgradeCost;
    public int newDamage;
    public float newAttackRate;
    public float newAttackRange;
    // Opcional: public Sprite newSprite;
}

public abstract class TowerWithBuffs : TowerBase // Supondo que TowerBase é sua classe raiz
{
    [Header("Configurações de Upgrade")]
    [SerializeField] protected List<TowerLevelData> upgradeLevels;

    // Nível atual da torre (0 = nível 1, 1 = nível 2, etc.)
    protected int currentLevel = 0;

    // --- Variáveis de Status ---
    // 'base' se refere ao status do nível atual, sem buffs.
    protected float baseAttackRate;
    protected int baseDamage;
    protected float baseAttackRange; // Se o alcance também for atualizável

    // 'original' se refere ao status inicial (Nível 1) antes de qualquer upgrade ou buff.
    // Usaremos 'base' como referência para buffs agora.

    private Coroutine currentBuffCoroutine;

    protected override void Start()
    {
        base.Start();

        // Aplica os status iniciais (Nível 1)
        ApplyLevelStats();
    }

    // --- NOVO: Lógica de Upgrade ---

    // Aplica os status do nível atual.
    private void ApplyLevelStats()
    {
        if (upgradeLevels != null && currentLevel < upgradeLevels.Count)
        {
            // Se for um upgrade (nível > 0)
            TowerLevelData data = upgradeLevels[currentLevel];
            baseDamage = data.newDamage;
            baseAttackRate = data.newAttackRate;
            baseAttackRange = data.newAttackRange;
        }
        else if (currentLevel == 0)
        {
            // Se for o nível inicial, pega os valores definidos na torre específica (Samurai, etc.)
            // Isso será feito no Start() da classe filha.
        }

        // Após aplicar o upgrade, restaura os status atuais para os novos valores base.
        // Isso garante que buffs não sejam perdidos durante um upgrade.
        attackRate = baseAttackRate;
        attackRange = baseAttackRange;
        HandleDamageBuff(1, false); // Restaura o dano para o novo 'baseDamage'
    }

    public void TryUpgrade()
    {
        // Verifica se já está no nível máximo
        if (currentLevel >= upgradeLevels.Count)
        {
            Debug.Log("Torre já está no nível máximo!");
            return;
        }

        int cost = upgradeLevels[currentLevel].upgradeCost;

        // Supondo que você tem um Totem.instance ou GameManager.instance para gerenciar a mana
        if (Totem.instance != null && Totem.instance.currentMana >= cost)
        {
            Totem.instance.SpendMana(cost);
            currentLevel++;
            ApplyLevelStats();
            Debug.Log($"{gameObject.name} atualizada para o Nível {currentLevel + 1}!");
        }
        else
        {
            Debug.Log("Mana insuficiente para o upgrade!");
        }
    }

    // --- Lógica de Buffs (Modificada) ---

    public void ApplyBuff(float damageMultiplier, float rateMultiplier, float duration)
    {
        if (currentBuffCoroutine != null)
        {
            StopCoroutine(currentBuffCoroutine);
            RemoveBuff();
        }
        currentBuffCoroutine = StartCoroutine(BuffSequence(damageMultiplier, rateMultiplier, duration));
    }

    private IEnumerator BuffSequence(float damageMultiplier, float rateMultiplier, float duration)
    {
        // Aplica o buff sobre os status BASE do nível atual
        attackRate = baseAttackRate * rateMultiplier;
        HandleDamageBuff(damageMultiplier, true); // O dano é aplicado sobre o 'baseDamage'

        yield return new WaitForSeconds(duration);

        RemoveBuff();
        currentBuffCoroutine = null;
    }

    public virtual void RemoveBuff()
    {
        // Restaura para os status BASE do nível atual
        attackRate = baseAttackRate;
        HandleDamageBuff(1, false); // O dano é restaurado para o 'baseDamage'
    }

    protected abstract void HandleDamageBuff(float multiplier, bool isApplying);
}