// TowerWithBuffs.cs (Com Sistema de Upgrades Integrado)
using System.Collections;
using System.Collections.Generic; // Para usar List<>
using UnityEngine;

// Defini��o da classe de dados para os n�veis de upgrade (coloque no mesmo arquivo)
[System.Serializable]
public class TowerLevelData
{
    public int upgradeCost;
    public int newDamage;
    public float newAttackRate;
    public float newAttackRange;
    // Opcional: public Sprite newSprite;
}

public abstract class TowerWithBuffs : TowerBase // Supondo que TowerBase � sua classe raiz
{
    [Header("Configura��es de Upgrade")]
    [SerializeField] protected List<TowerLevelData> upgradeLevels;

    // N�vel atual da torre (0 = n�vel 1, 1 = n�vel 2, etc.)
    protected int currentLevel = 0;

    // --- Vari�veis de Status ---
    // 'base' se refere ao status do n�vel atual, sem buffs.
    protected float baseAttackRate;
    protected int baseDamage;
    protected float baseAttackRange; // Se o alcance tamb�m for atualiz�vel

    // 'original' se refere ao status inicial (N�vel 1) antes de qualquer upgrade ou buff.
    // Usaremos 'base' como refer�ncia para buffs agora.

    private Coroutine currentBuffCoroutine;

    protected override void Start()
    {
        base.Start();

        // Aplica os status iniciais (N�vel 1)
        ApplyLevelStats();
    }

    // --- NOVO: L�gica de Upgrade ---

    // Aplica os status do n�vel atual.
    private void ApplyLevelStats()
    {
        if (upgradeLevels != null && currentLevel < upgradeLevels.Count)
        {
            // Se for um upgrade (n�vel > 0)
            TowerLevelData data = upgradeLevels[currentLevel];
            baseDamage = data.newDamage;
            baseAttackRate = data.newAttackRate;
            baseAttackRange = data.newAttackRange;
        }
        else if (currentLevel == 0)
        {
            // Se for o n�vel inicial, pega os valores definidos na torre espec�fica (Samurai, etc.)
            // Isso ser� feito no Start() da classe filha.
        }

        // Ap�s aplicar o upgrade, restaura os status atuais para os novos valores base.
        // Isso garante que buffs n�o sejam perdidos durante um upgrade.
        attackRate = baseAttackRate;
        attackRange = baseAttackRange;
        HandleDamageBuff(1, false); // Restaura o dano para o novo 'baseDamage'
    }

    public void TryUpgrade()
    {
        // Verifica se j� est� no n�vel m�ximo
        if (currentLevel >= upgradeLevels.Count)
        {
            Debug.Log("Torre j� est� no n�vel m�ximo!");
            return;
        }

        int cost = upgradeLevels[currentLevel].upgradeCost;

        // Supondo que voc� tem um Totem.instance ou GameManager.instance para gerenciar a mana
        if (Totem.instance != null && Totem.instance.currentMana >= cost)
        {
            Totem.instance.SpendMana(cost);
            currentLevel++;
            ApplyLevelStats();
            Debug.Log($"{gameObject.name} atualizada para o N�vel {currentLevel + 1}!");
        }
        else
        {
            Debug.Log("Mana insuficiente para o upgrade!");
        }
    }

    // --- L�gica de Buffs (Modificada) ---

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
        // Aplica o buff sobre os status BASE do n�vel atual
        attackRate = baseAttackRate * rateMultiplier;
        HandleDamageBuff(damageMultiplier, true); // O dano � aplicado sobre o 'baseDamage'

        yield return new WaitForSeconds(duration);

        RemoveBuff();
        currentBuffCoroutine = null;
    }

    public virtual void RemoveBuff()
    {
        // Restaura para os status BASE do n�vel atual
        attackRate = baseAttackRate;
        HandleDamageBuff(1, false); // O dano � restaurado para o 'baseDamage'
    }

    protected abstract void HandleDamageBuff(float multiplier, bool isApplying);
}