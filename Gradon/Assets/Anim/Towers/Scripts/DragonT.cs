// DragonT.cs (Vers�o CORRETA e LIMPA)

using UnityEngine;

// --- CORRE��O PRINCIPAL ---
// A classe agora herda de 'TowerWithBuffs', que � uma classe filha de 'TowerBase'.
// Isso a torna uma "torre v�lida" para o sistema e permite que ela use a l�gica
// de busca de alvos e timer de ataque da classe m�e.
public class DragonT : TowerWithBuffs
{
    [Header("Atributos do Drag�o (N�vel 1)")]
    [SerializeField] private int initialDamage = 15;
    [SerializeField] private float initialAttackRate = 0.8f;
    [SerializeField] private float initialAttackRange = 7.0f;

    [Header("Refer�ncias do Proj�til")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    // Vari�vel interna para o dano atual, que ser� atualizada por buffs/upgrades
    private int damage;

    // --- L�GICA DE HERAN�A ---
    // Removemos Update(), UpdateTarget(), currentTarget e attackCountdown daqui,
    // pois a classe m�e 'TowerBase' j� cuida de tudo isso para n�s!

    /// <summary>
    /// Configura os status iniciais da torre no momento em que ela � criada.
    /// </summary>
    protected override void Start()
    {
        // Define os status base que ser�o usados pela classe m�e
        baseDamage = initialDamage;
        baseAttackRate = initialAttackRate;
        baseAttackRange = initialAttackRange;

        // Chama o m�todo Start() da classe m�e para aplicar esses status
        base.Start();
    }

    /// <summary>
    /// Define como o dano desta torre reage a um buff (exigido pela classe m�e).
    /// </summary>
    protected override void HandleDamageBuff(float multiplier, bool isApplying)
    {
        damage = isApplying ? Mathf.RoundToInt(baseDamage * multiplier) : baseDamage;
    }

    /// <summary>
    /// L�gica de ataque espec�fica do Drag�o. � chamada AUTOMATICAMENTE pela classe m�e.
    /// </summary>
    protected override void Attack()
    {
        // Checagens de seguran�a
        if (projectilePrefab == null || firePoint == null || currentTarget == null) return;

        // Cria o proj�til
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Configura o proj�til
        Projectile projectileScript = projectileGO.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            // Passa o alvo (encontrado pela classe m�e) e o dano atual para o proj�til
            projectileScript.Seek(currentTarget, this.damage);
        }
    }

    /// <summary>
    /// Desenha o raio de alcance da torre no Editor da Unity.
    /// </summary>
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Usa a vari�vel 'attackRange' da classe m�e, que � atualizada por buffs/upgrades
        Gizmos.DrawWireSphere(transform.position, Application.isPlaying ? attackRange : initialAttackRange);
    }
}