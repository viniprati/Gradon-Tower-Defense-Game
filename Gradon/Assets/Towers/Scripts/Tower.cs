// TowerBase.cs
using UnityEngine;

public abstract class TowerBase : MonoBehaviour
{
    [Header("Atributos Gerais da Torre")]
    [SerializeField] protected float attackRange = 5f;
    [SerializeField] protected float attackRate = 1f;
    [SerializeField] public int cost = 50;
    [SerializeField] public Sprite towerIcon;

    [Header("Referências (Setup no Prefab)")]
    [Tooltip("A parte da torre que deve girar para encarar o inimigo.")]
    [SerializeField] protected Transform partToRotate;
    [Tooltip("O objeto SpriteRenderer que visualmente representa o alcance da torre.")]
    [SerializeField] protected SpriteRenderer rangeIndicator; // MUDANÇA: Agora é um SpriteRenderer direto.

    [Header("Configurações Visuais")]
    [Tooltip("A opacidade do indicador de alcance (0 = invisível, 1 = opaco).")]
    [Range(0f, 1f)]
    [SerializeField] private float rangeIndicatorOpacity = 0.15f; // Opacidade padrão de 15%

    // --- Variáveis Internas ---
    protected Transform currentTarget;
    protected float attackCooldown = 0f;
    private static readonly int EnemyLayer = 1 << 6; // Layer "Enemy"

    protected virtual void Start()
    {
        // Garante que o indicador de alcance esteja configurado corretamente
        if (rangeIndicator != null)
        {
            // O indicador agora fica SEMPRE ATIVO
            rangeIndicator.gameObject.SetActive(true);

            // Ajusta a escala do indicador para corresponder ao alcance
            // A escala deve ser o dobro do alcance, pois a escala é baseada no diâmetro.
            float diameter = attackRange * 2f;
            rangeIndicator.transform.localScale = new Vector3(diameter, diameter, 1f);

            // AJUSTA A OPACIDADE
            Color currentColor = rangeIndicator.color;
            rangeIndicator.color = new Color(currentColor.r, currentColor.g, currentColor.b, rangeIndicatorOpacity);
        }
        attackCooldown = 0f;
    }

    protected virtual void Update()
    {
        if (currentTarget == null || Vector3.Distance(transform.position, currentTarget.position) > attackRange)
        {
            FindTarget();
        }

        if (currentTarget != null)
        {
            HandleRotation();
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                Attack();
                attackCooldown = 1f / attackRate;
            }
        }
    }

    private void FindTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, EnemyLayer);
        float closestDistance = Mathf.Infinity;
        Transform newTarget = null;

        foreach (Collider2D collider in colliders)
        {
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                newTarget = collider.transform;
            }
        }
        currentTarget = newTarget;
    }

    private void HandleRotation()
    {
        if (partToRotate == null) return;
        Vector2 direction = currentTarget.position - partToRotate.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        partToRotate.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected abstract void Attack();

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}