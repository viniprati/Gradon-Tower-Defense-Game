// DragonT.cs (Corrigido com inicialização do dano)

using UnityEngine;

public class DragonT : TowerWithBuffs
{
    [Header("Atributos do Dragão (Nível 1)")]
    [SerializeField] private int initialDamage = 25;
    [SerializeField] private float initialAttackRate = 0.8f;
    [SerializeField] private float initialAttackRange = 7.0f;

    [Header("Referências do Projétil")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    private int damage; // Variável interna para o dano atual

    /// <summary>
    /// Configura os status iniciais da torre.
    /// </summary>
    protected override void Start()
    {
        // Define os status base para o nível inicial
        baseDamage = initialDamage;
        baseAttackRate = initialAttackRate;
        baseAttackRange = initialAttackRange;

        // Chama o Start da classe mãe para aplicar esses status
        base.Start();

        // --- CORREÇÃO ADICIONADA AQUI ---
        // Inicializa o dano atual ('damage') com o dano base ('baseDamage').
        // Sem esta linha, 'damage' começaria com valor 0.
        this.damage = baseDamage;
    }

    /// <summary>
    /// Define como o dano desta torre reage a um buff.
    /// </summary>
    protected override void HandleDamageBuff(float multiplier, bool isApplying)
    {
        // Se estiver aplicando, multiplica o dano base. Se não, restaura ao dano base.
        damage = isApplying ? Mathf.RoundToInt(baseDamage * multiplier) : baseDamage;
    }

    /// <summary>
    /// Lógica de ataque específica do Dragão.
    /// </summary>
    protected override void Attack()
    {
        if (projectilePrefab == null || firePoint == null || currentTarget == null) return;

        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = projectileGO.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            // Agora 'this.damage' sempre terá um valor correto, com ou sem buff.
            projectileScript.Seek(currentTarget, this.damage);
        }
    }

    /// <summary>
    /// Desenha o raio de alcance da torre no Editor.
    /// </summary>
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Application.isPlaying ? attackRange : initialAttackRange);
    }
}