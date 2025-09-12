// DragonT.cs (Versão CORRETA e LIMPA)

using UnityEngine;

// --- CORREÇÃO PRINCIPAL ---
// A classe agora herda de 'TowerWithBuffs', que é uma classe filha de 'TowerBase'.
// Isso a torna uma "torre válida" para o sistema e permite que ela use a lógica
// de busca de alvos e timer de ataque da classe mãe.
public class DragonT : TowerWithBuffs
{
    [Header("Atributos do Dragão (Nível 1)")]
    [SerializeField] private int initialDamage = 25; // dano alterado para 25
    [SerializeField] private float initialAttackRate = 0.8f;
    [SerializeField] private float initialAttackRange = 7.0f;

    [Header("Referências do Projétil")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    // Variável interna para o dano atual, que será atualizada por buffs/upgrades
    private int damage;

    // --- LÓGICA DE HERANÇA ---
    // Removemos Update(), UpdateTarget(), currentTarget e attackCountdown daqui,
    // pois a classe mãe 'TowerBase' já cuida de tudo isso para nós!

    /// <summary>
    /// Configura os status iniciais da torre no momento em que ela é criada.
    /// </summary>
    protected override void Start()
    {
        // Define os status base que serão usados pela classe mãe
        baseDamage = initialDamage;
        baseAttackRate = initialAttackRate;
        baseAttackRange = initialAttackRange;

        // Chama o método Start() da classe mãe para aplicar esses status
        base.Start();
    }

    /// <summary>
    /// Define como o dano desta torre reage a um buff (exigido pela classe mãe).
    /// </summary>
    protected override void HandleDamageBuff(float multiplier, bool isApplying)
    {
        damage = isApplying ? Mathf.RoundToInt(baseDamage * multiplier) : baseDamage;
    }

    /// <summary>
    /// Lógica de ataque específica do Dragão. É chamada AUTOMATICAMENTE pela classe mãe.
    /// </summary>
    protected override void Attack()
    {
        // Checagens de segurança
        if (projectilePrefab == null || firePoint == null || currentTarget == null) return;

        // Cria o projétil
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Configura o projétil
        Projectile projectileScript = projectileGO.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            // Passa o alvo (encontrado pela classe mãe) e o dano atual para o projétil
            projectileScript.Seek(currentTarget, this.damage);
        }
    }

    /// <summary>
    /// Desenha o raio de alcance da torre no Editor da Unity.
    /// </summary>
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Usa a variável 'attackRange' da classe mãe, que é atualizada por buffs/upgrades
        Gizmos.DrawWireSphere(transform.position, Application.isPlaying ? attackRange : initialAttackRange);
    }
}