// DragonT.cs (Corrigido com inicializa��o do dano)

using UnityEngine;

public class DragonT : TowerWithBuffs
{
    [Header("Atributos do Drag�o (N�vel 1)")]
    [SerializeField] private int initialDamage = 25;
    [SerializeField] private float initialAttackRate = 0.8f;
    [SerializeField] private float initialAttackRange = 7.0f;

    [Header("Refer�ncias do Proj�til")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    private int damage; // Vari�vel interna para o dano atual

    /// <summary>
    /// Configura os status iniciais da torre.
    /// </summary>
    protected override void Start()
    {
        // Define os status base para o n�vel inicial
        baseDamage = initialDamage;
        baseAttackRate = initialAttackRate;
        baseAttackRange = initialAttackRange;

        // Chama o Start da classe m�e para aplicar esses status
        base.Start();

        // --- CORRE��O ADICIONADA AQUI ---
        // Inicializa o dano atual ('damage') com o dano base ('baseDamage').
        // Sem esta linha, 'damage' come�aria com valor 0.
        this.damage = baseDamage;
    }

    /// <summary>
    /// Define como o dano desta torre reage a um buff.
    /// </summary>
    protected override void HandleDamageBuff(float multiplier, bool isApplying)
    {
        // Se estiver aplicando, multiplica o dano base. Se n�o, restaura ao dano base.
        damage = isApplying ? Mathf.RoundToInt(baseDamage * multiplier) : baseDamage;
    }

    /// <summary>
    /// L�gica de ataque espec�fica do Drag�o.
    /// </summary>
    protected override void Attack()
    {
        if (projectilePrefab == null || firePoint == null || currentTarget == null) return;

        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = projectileGO.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            // Agora 'this.damage' sempre ter� um valor correto, com ou sem buff.
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