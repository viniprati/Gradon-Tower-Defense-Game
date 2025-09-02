// KirinT.cs
using UnityEngine;
using System.Collections.Generic;

// --- CORREÇÃO PRINCIPAL: Herda de TowerWithBuffs ---
// Agora esta classe é reconhecida como uma torre válida pelo seu sistema
// e ganha acesso a Custo, Upgrades e outras funcionalidades base.
public class KirinT : TowerWithBuffs
{
    [Header("Atributos do Buff do Kirin")]
    // [SerializeField] private float buffRadius = 4f; // Comentado: Usaremos o 'attackRange' da classe base
    [SerializeField] private float attackRateMultiplier = 1.5f;
    [SerializeField] private float checkInterval = 1.0f; // Intervalo para checar torres próximas

    private List<TowerWithBuffs> buffedTowers = new List<TowerWithBuffs>();

    /// <summary>
    /// No Start, definimos os valores base e iniciamos a rotina de buff.
    /// </summary>
    protected override void Start()
    {
        // Define os valores para a classe base. Como não ataca, dano e cadência são 0.
        baseDamage = 0;
        baseAttackRate = 0;
        // O alcance do buff agora será o 'initialAttackRange' que você define no Inspector.
        baseAttackRange = initialAttackRange;

        // Chama o Start da classe mãe para aplicar os status
        base.Start();

        // Inicia a rotina que aplica o buff a cada 'checkInterval' segundos
        InvokeRepeating("ApplyBuffToNearbyTowers", 0f, checkInterval);
    }

    /// <summary>
    /// Procura por torres dentro do alcance e aplica/remove buffs.
    /// </summary>
    private void ApplyBuffToNearbyTowers()
    {
        // Limpa a lista de torres que foram destruídas
        buffedTowers.RemoveAll(tower => tower == null);

        // Cria uma cópia da lista para poder modificar a original durante o loop
        List<TowerWithBuffs> towersCurrentlyBuffed = new List<TowerWithBuffs>(buffedTowers);

        // Checa se as torres que *estavam* com buff ainda estão no alcance
        foreach (TowerWithBuffs tower in towersCurrentlyBuffed)
        {
            // A variável 'attackRange' vem da classe base e pode ser atualizada por upgrades.
            if (Vector2.Distance(transform.position, tower.transform.position) > attackRange)
            {
                tower.RemoveBuff();
                buffedTowers.Remove(tower);
            }
        }

        // Procura por novas torres que entraram no alcance
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Towers"));

        foreach (var col in colliders)
        {
            TowerWithBuffs tower = col.GetComponent<TowerWithBuffs>();
            // Se encontrou uma torre válida, que não seja ela mesma e que ainda não está na lista
            if (tower != null && tower != this && !buffedTowers.Contains(tower))
            {
                // Aplica o buff (supondo que ApplyBuff não precisa de dano, apenas rate)
                // O seu TowerWithBuffs tem um ApplyBuff que aceita dano, rate e duração.
                // Vamos usar esse método.
                tower.ApplyBuff(1f, attackRateMultiplier, checkInterval + 1f); // Duração um pouco maior que o intervalo
                buffedTowers.Add(tower);
            }
        }
    }

    /// <summary>
    /// Garante que os buffs sejam removidos se a torre Kirin for destruída/vendida.
    /// </summary>
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

    /// <summary>
    /// O Kirin não ataca, então este método obrigatório fica vazio.
    /// </summary>
    protected override void Attack() { }

    /// <summary>
    /// O Kirin não tem dano próprio, então este método obrigatório fica vazio.
    /// </summary>
    protected override void HandleDamageBuff(float multiplier, bool isApplying) { }

    // OnDrawGizmosSelected já é herdado da classe base (TowerBase), então ele desenhará
    // o 'attackRange' automaticamente. Não precisamos reescrevê-lo.
}