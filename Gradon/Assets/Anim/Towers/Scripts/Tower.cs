using UnityEngine;

// A classe agora � 'abstract' para garantir que ela sirva apenas como um molde.
// Voc� n�o poder� adicionar o componente "Tower" diretamente a um objeto.
// Em vez disso, voc� adicionar� scripts como "DragonT", que herdam dele.
public abstract class Tower : MonoBehaviour
{
    [Header("Informa��es da Torre")]
    public string towerName = "Torre Padr�o";
    public int cost = 50;
    public Sprite towerIcon;

    [Header("Atributos Gerais da Torre")]
    public float attackRange = 7f;
    public float attackRate = 1f; // Ataques por segundo
    public int attackDamage = 50;

    [Header("Configura��o de Alvo")]
    [SerializeField] protected string enemyTag = "Enemy";

    [Header("Refer�ncias (Setup no Prefab)")]
    [SerializeField] protected Transform partToRotate;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected SpriteRenderer rangeIndicator;

    // Vari�veis internas
    protected Transform currentTarget;
    protected float attackCooldown = 0f;
    protected float originalAttackRate; // Para o sistema de buffs

    protected virtual void Start()
    {
        originalAttackRate = attackRate;
        attackCooldown = 1f / attackRate; // Permite atirar quase imediatamente
        if (rangeIndicator != null) rangeIndicator.transform.localScale = new Vector3(attackRange * 2, attackRange * 2, 1);
        ShowRangeIndicator(false);
    }

    protected virtual void Update()
    {
        if (currentTarget == null || Vector2.Distance(transform.position, currentTarget.position) > attackRange)
        {
            FindTarget();
        }

        attackCooldown -= Time.deltaTime;

        if (currentTarget != null)
        {
            HandleRotation();

            if (attackCooldown <= 0f)
            {
                Attack();
                attackCooldown = 1f / attackRate;
            }
        }
    }

    private void FindTarget()
    {
        // Este m�todo � mais eficiente que FindGameObjectsWithTag se todos os inimigos tiverem o script "Enemy"
        Enemy[] enemies = FindObjectsOfType<Enemy>();
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

        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            currentTarget = nearestEnemy.transform;
        }
        else
        {
            currentTarget = null;
        }
    }

    protected virtual void HandleRotation()
    {
        if (partToRotate == null) return;

        Vector2 direction = currentTarget.position - partToRotate.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        // Usando Slerp para uma rota��o mais suave
        partToRotate.rotation = Quaternion.Slerp(partToRotate.rotation, rotation, Time.deltaTime * 10f);
    }

    /// <summary>
    /// O m�todo de ataque padr�o. Torres espec�ficas podem sobrescrev�-lo para comportamentos �nicos.
    /// </summary>
    protected virtual void Attack()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError(towerName + " n�o tem um proj�til ou ponto de tiro configurado!");
            return;
        }

        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            // Passa o alvo e o dano para o proj�til
            projectile.Seek(currentTarget, attackDamage);
        }
    }

    /// <summary>
    /// Mostra ou esconde o c�rculo de alcance.
    /// </summary>
    public void ShowRangeIndicator(bool isVisible)
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.enabled = isVisible;
        }
    }

    // M�todos para o sistema de buffs
    public virtual void ApplyBuff(float rateMultiplier) { attackRate = originalAttackRate * rateMultiplier; }
    public virtual void RemoveBuff() { attackRate = originalAttackRate; }

    // Desenha o Gizmo de alcance no editor
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}