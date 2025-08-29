// SamuraiTower.cs (Corrigido)

using UnityEngine;

// A classe herda de TowerWithBuffs. Isso significa que ela AUTOMATICAMENTE ganha
// a habilidade de procurar alvos usando a 'enemyTag' definida na classe mãe.
// Não precisamos reescrever essa lógica aqui.
public class SamuraiTower : TowerWithBuffs
{
    [Header("Atributos Samurai (Nível 1)")]
    [SerializeField] private int initialDamage = 15;
    [SerializeField] private float initialAttackRate = 1.0f;
    [SerializeField] private float initialAttackRange = 2.0f;

    [Header("Efeitos Visuais")]
    [Tooltip("Prefab do efeito de corte/ataque a ser criado no alvo.")]
    [SerializeField] private GameObject attackEffectPrefab;

    // Variável interna para guardar o dano atual, já considerando buffs e upgrades.
    private int damage;

    /// <summary>
    /// Configura os status iniciais da torre quando ela é criada na cena.
    /// </summary>
    protected override void Start()
    {
        // Define os status BASE da torre com os valores iniciais.
        baseDamage = initialDamage;
        baseAttackRate = initialAttackRate;
        baseAttackRange = initialAttackRange;

        // Chama o Start() da classe mãe (TowerWithBuffs).
        // É aqui que a mágica acontece: a classe base aplica os status e prepara a lógica de busca de alvo.
        base.Start();
    }

    /// <summary>
    /// Define como o dano desta torre reage a um buff.
    /// </summary>
    protected override void HandleDamageBuff(float multiplier, bool isApplying)
    {
        damage = isApplying ? Mathf.RoundToInt(baseDamage * multiplier) : baseDamage;
    }

    /// <summary>
    /// Contém a lógica de ataque específica do Samurai.
    /// Este método é chamado automaticamente pela classe base 'Tower.cs' quando ela encontra um alvo
    /// e o timer de ataque permite.
    /// </summary>
    protected override void Attack()
    {
        // Checagem de segurança para garantir que o alvo ainda existe antes de atacar.
        if (currentTarget == null) return;

        // Cria o efeito visual do ataque na posição do alvo para um impacto melhor.
        if (attackEffectPrefab != null)
        {
            Instantiate(attackEffectPrefab, currentTarget.position, Quaternion.identity);
        }

        // Pega o componente 'Enemy' diretamente do nosso alvo atual.
        Enemy enemy = currentTarget.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Causa dano APENAS ao nosso alvo focado.
            enemy.TakeDamage(damage);
        }
    }

    /// <summary>
    /// Desenha o raio de alcance da torre no Editor da Unity para facilitar o posicionamento.
    /// </summary>
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; // Cor diferente para distinguir do dragão

        // Se o jogo não estiver rodando, usa o valor inicial. Se estiver, usa o valor atual.
        if (Application.isPlaying)
        {
            // --- CORREÇÃO AQUI ---
            // A variável que guarda o alcance ATUAL da torre está na classe mãe (TowerBase)
            // e se chama 'attackRange'. Ela é 'protected', então podemos usá-la aqui.
            Gizmos.DrawWireSphere(transform.position, attackRange); // MUDADO DE currentAttackRange
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, initialAttackRange);
        }
    }
}