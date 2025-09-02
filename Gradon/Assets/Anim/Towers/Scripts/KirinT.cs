// KirinT.cs
using UnityEngine;
using System.Collections.Generic;

// --- CORRE��O PRINCIPAL: Herda de TowerWithBuffs ---
// Agora esta classe � reconhecida como uma torre v�lida pelo seu sistema
// e ganha acesso a Custo, Upgrades e outras funcionalidades base.
public class KirinT : TowerWithBuffs
{
    [Header("Atributos do Buff do Kirin")]
    // [SerializeField] private float buffRadius = 4f; // Comentado: Usaremos o 'attackRange' da classe base
    [SerializeField] private float attackRateMultiplier = 1.5f;
    [SerializeField] private float checkInterval = 1.0f; // Intervalo para checar torres pr�ximas

    private List<TowerWithBuffs> buffedTowers = new List<TowerWithBuffs>();

    /// <summary>
    /// No Start, definimos os valores base e iniciamos a rotina de buff.
    /// </summary>
    protected override void Start()
    {
        // Define os valores para a classe base. Como n�o ataca, dano e cad�ncia s�o 0.
        baseDamage = 0;
        baseAttackRate = 0;
        // O alcance do buff agora ser� o 'initialAttackRange' que voc� define no Inspector.
        baseAttackRange = initialAttackRange;

        // Chama o Start da classe m�e para aplicar os status
        base.Start();

        // Inicia a rotina que aplica o buff a cada 'checkInterval' segundos
        InvokeRepeating("ApplyBuffToNearbyTowers", 0f, checkInterval);
    }

    /// <summary>
    /// Procura por torres dentro do alcance e aplica/remove buffs.
    /// </summary>
    private void ApplyBuffToNearbyTowers()
    {
        // Limpa a lista de torres que foram destru�das
        buffedTowers.RemoveAll(tower => tower == null);

        // Cria uma c�pia da lista para poder modificar a original durante o loop
        List<TowerWithBuffs> towersCurrentlyBuffed = new List<TowerWithBuffs>(buffedTowers);

        // Checa se as torres que *estavam* com buff ainda est�o no alcance
        foreach (TowerWithBuffs tower in towersCurrentlyBuffed)
        {
            // A vari�vel 'attackRange' vem da classe base e pode ser atualizada por upgrades.
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
            // Se encontrou uma torre v�lida, que n�o seja ela mesma e que ainda n�o est� na lista
            if (tower != null && tower != this && !buffedTowers.Contains(tower))
            {
                // Aplica o buff (supondo que ApplyBuff n�o precisa de dano, apenas rate)
                // O seu TowerWithBuffs tem um ApplyBuff que aceita dano, rate e dura��o.
                // Vamos usar esse m�todo.
                tower.ApplyBuff(1f, attackRateMultiplier, checkInterval + 1f); // Dura��o um pouco maior que o intervalo
                buffedTowers.Add(tower);
            }
        }
    }

    /// <summary>
    /// Garante que os buffs sejam removidos se a torre Kirin for destru�da/vendida.
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
    /// O Kirin n�o ataca, ent�o este m�todo obrigat�rio fica vazio.
    /// </summary>
    protected override void Attack() { }

    /// <summary>
    /// O Kirin n�o tem dano pr�prio, ent�o este m�todo obrigat�rio fica vazio.
    /// </summary>
    protected override void HandleDamageBuff(float multiplier, bool isApplying) { }

    // OnDrawGizmosSelected j� � herdado da classe base (TowerBase), ent�o ele desenhar�
    // o 'attackRange' automaticamente. N�o precisamos reescrev�-lo.
}