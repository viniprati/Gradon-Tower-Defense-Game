// SamuraiTower.cs (Corrigido)

using UnityEngine;

// A classe herda de TowerWithBuffs. Isso significa que ela AUTOMATICAMENTE ganha
// a habilidade de procurar alvos usando a 'enemyTag' definida na classe m�e.
// N�o precisamos reescrever essa l�gica aqui.
public class SamuraiTower : TowerWithBuffs
{
    [Header("Atributos Samurai (N�vel 1)")]
    [SerializeField] private int initialDamage = 15;
    [SerializeField] private float initialAttackRate = 1.0f;
    [SerializeField] private float initialAttackRange = 2.0f;

    [Header("Efeitos Visuais")]
    [Tooltip("Prefab do efeito de corte/ataque a ser criado no alvo.")]
    [SerializeField] private GameObject attackEffectPrefab;

    // Vari�vel interna para guardar o dano atual, j� considerando buffs e upgrades.
    private int damage;

    /// <summary>
    /// Configura os status iniciais da torre quando ela � criada na cena.
    /// </summary>
    protected override void Start()
    {
        // Define os status BASE da torre com os valores iniciais.
        baseDamage = initialDamage;
        baseAttackRate = initialAttackRate;
        baseAttackRange = initialAttackRange;

        // Chama o Start() da classe m�e (TowerWithBuffs).
        // � aqui que a m�gica acontece: a classe base aplica os status e prepara a l�gica de busca de alvo.
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
    /// Cont�m a l�gica de ataque espec�fica do Samurai.
    /// Este m�todo � chamado automaticamente pela classe base 'Tower.cs' quando ela encontra um alvo
    /// e o timer de ataque permite.
    /// </summary>
    protected override void Attack()
    {
        // Checagem de seguran�a para garantir que o alvo ainda existe antes de atacar.
        if (currentTarget == null) return;

        // Cria o efeito visual do ataque na posi��o do alvo para um impacto melhor.
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
        Gizmos.color = Color.yellow; // Cor diferente para distinguir do drag�o

        // Se o jogo n�o estiver rodando, usa o valor inicial. Se estiver, usa o valor atual.
        if (Application.isPlaying)
        {
            // --- CORRE��O AQUI ---
            // A vari�vel que guarda o alcance ATUAL da torre est� na classe m�e (TowerBase)
            // e se chama 'attackRange'. Ela � 'protected', ent�o podemos us�-la aqui.
            Gizmos.DrawWireSphere(transform.position, attackRange); // MUDADO DE currentAttackRange
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, initialAttackRange);
        }
    }
}