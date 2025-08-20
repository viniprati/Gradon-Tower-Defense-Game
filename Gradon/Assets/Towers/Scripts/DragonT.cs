// RangedTower.cs (Integrado com Upgrades e Buffs)
using UnityEngine;

// Garante que a torre herda da classe correta que lida com buffs e upgrades
public class RangedTower : TowerWithBuffs
{
    [Header("Atributos Ranged (Nível 1)")]
    // Estes são os valores iniciais da torre no Nível 1
    [SerializeField] private int initialDamage = 10;
    [SerializeField] private float initialAttackRate = 1.0f;
    [SerializeField] private float initialAttackRange = 5.0f;

    [Header("Referências do Projétil")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    // A variável 'damage' agora representa o dano ATUAL (com upgrades e buffs)
    private int damage;

    // O método Start define os valores BASE para o Nível 1
    protected override void Start()
    {
        // Define os status BASE para o nível inicial.
        // Estes valores serão atualizados permanentemente pelos upgrades.
        baseDamage = initialDamage;
        baseAttackRate = initialAttackRate;
        baseAttackRange = initialAttackRange;

        // Chama o Start da classe base (TowerWithBuffs), que irá aplicar esses status.
        base.Start();
    }

    // Este método é chamado pela classe base para aplicar ou remover buffs de dano.
    // Ele trabalha com 'baseDamage' para calcular o dano com buff.
    protected override void HandleDamageBuff(float multiplier, bool isApplying)
    {
        // Se estiver aplicando, multiplica o dano base. Se não, restaura para o dano base.
        damage = isApplying ? Mathf.RoundToInt(baseDamage * multiplier) : baseDamage;
    }

    // A lógica de ataque dispara um projétil com o dano atual.
    protected override void Attack()
    {
        if (projectilePrefab == null || firePoint == null || currentTarget == null) return;

        // Cria o projétil no ponto de disparo
        GameObject projGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Pega o script do projétil para configurá-lo
        Projectile projectileScript = projGO.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            // Passa o alvo e o dano ATUAL (que já inclui upgrades e buffs) para o projétil.
            // Supondo que seu projétil tenha um método Seek ou Launch.
            // projectileScript.Seek(currentTarget, this.damage);
        }

        Debug.Log(gameObject.name + " atirou em " + currentTarget.name + " com " + this.damage + " de dano.");
    }
}