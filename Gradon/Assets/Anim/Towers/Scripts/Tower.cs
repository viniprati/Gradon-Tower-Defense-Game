// TowerBase.cs (Completo e Corrigido)
using UnityEngine;

public abstract class TowerBase : MonoBehaviour
{
    [Header("Informações da Torre")]
    public string towerName = "Torre Padrão";

    [Header("Atributos Gerais da Torre")]
    public float attackRange = 5f;
    public float attackRate = 1f;
    public int cost = 50;
    public Sprite towerIcon;

    [Header("Configuração de Alvo")]
    [SerializeField] protected string enemyTag = "Enemy";

    [Header("Referências (Setup no Prefab)")]
    [SerializeField] protected Transform partToRotate; // A parte que gira
    [SerializeField] protected SpriteRenderer rangeIndicator; // O círculo visual

    // Variáveis internas
    protected Transform currentTarget;
    protected float attackCooldown = 0f;
    protected float originalAttackRate; // Para buffs

    protected virtual void Start()
    {
        originalAttackRate = attackRate;
        attackCooldown = 1f / attackRate; // Para atirar quase imediatamente
        ShowRangeIndicator(false);
    }

    /// <summary>
    /// O cérebro da torre: procura alvos e controla o cooldown de ataque.
    /// </summary>
    protected virtual void Update()
    {
        // Se não temos um alvo ou se o alvo saiu do alcance, procuramos um novo.
        if (currentTarget == null || Vector2.Distance(transform.position, currentTarget.position) > attackRange)
        {
            FindTarget();
        }

        // Diminui o tempo de recarga a cada segundo.
        attackCooldown -= Time.deltaTime;

        // Se temos um alvo válido e o tempo de recarga acabou...
        if (currentTarget != null)
        {
            HandleRotation(); // Mira no alvo

            if (attackCooldown <= 0f)
            {
                Attack(); // Ataca!
                attackCooldown = 1f / attackRate; // Reseta o tempo de recarga.
            }
        }
    }

    /// <summary>
    /// Escaneia a área em busca do inimigo mais próximo.
    /// </summary>
    private void FindTarget()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>(); // Encontra todos os inimigos na cena
        float shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        foreach (Enemy enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        // Se encontrou um inimigo e ele está dentro do alcance, define como alvo.
        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            currentTarget = nearestEnemy.transform;
        }
        else
        {
            currentTarget = null; // Se não, limpa o alvo.
        }
    }

    /// <summary>
    /// Gira a parte móvel da torre para encarar o alvo.
    /// </summary>
    private void HandleRotation()
    {
        if (partToRotate == null) return;

        Vector2 direction = currentTarget.position - partToRotate.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Adicione -90f se o seu sprite estiver "deitado"
        partToRotate.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    /// <summary>
    /// O método de ataque que cada torre específica (Dragão, Samurai) deve implementar.
    /// </summary>
    protected abstract void Attack();

    /// <summary>
    /// Mostra ou esconde o círculo de alcance visual.
    /// </summary>
    public void ShowRangeIndicator(bool isVisible)
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.enabled = isVisible;
        }
    }

    // Métodos para o sistema de buffs
    public virtual void ApplyBuff(float rateMultiplier) { attackRate = originalAttackRate * rateMultiplier; }
    public virtual void RemoveBuff() { attackRate = originalAttackRate; }

    // Desenha o Gizmo de alcance no editor
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}