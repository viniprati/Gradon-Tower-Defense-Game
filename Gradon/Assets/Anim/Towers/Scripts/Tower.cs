// TowerBase.cs
using Mono.Cecil.Cil;
using UnityEngine;
public abstract class TowerBase : MonoBehaviour
{
    [Header("Atributos Gerais da Torre")]
    [SerializeField] protected float attackRange = 5f;
    [SerializeField] protected float attackRate = 1f;
    [SerializeField] public int cost = 50;
    [SerializeField] public Sprite towerIcon;
    
    // --- MUDAN�A #1: ADICIONADO CAMPO PARA A TAG DO INIMIGO ---
    // Adicionamos esta vari�vel para que CADA torre possa saber qual tag procurar.
    // Isso centraliza a l�gica e nos permite corrigir o erro diretamente no Inspector.
    [Header("Configura��o de Alvo")]
[Tooltip("A tag que esta torre ir� procurar para atacar.")]
    [SerializeField] protected string enemyTag = "Enemy";

    [Header("Refer�ncias (Setup no Prefab)")]
    [Tooltip("A parte da torre que deve girar para encarar o inimigo.")]
    [SerializeField] protected Transform partToRotate;
    [Tooltip("O objeto SpriteRenderer que visualmente representa o alcance da torre.")]
    [SerializeField] protected SpriteRenderer rangeIndicator;

    [Header("Configura��es Visuais")]
    [Tooltip("A opacidade do indicador de alcance (0 = invis�vel, 1 = opaco).")]
    [Range(0f, 1f)]
    [SerializeField] private float rangeIndicatorOpacity = 0.15f;

    // --- Vari�veis Internas ---
    protected Transform currentTarget;
    protected float attackCooldown = 0f;
    protected float originalAttackRate;

    protected virtual void Start()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.gameObject.SetActive(true);
            float diameter = attackRange * 2f;
            rangeIndicator.transform.localScale = new Vector3(diameter, diameter, 1f);
            Color currentColor = rangeIndicator.color;
            rangeIndicator.color = new Color(currentColor.r, currentColor.g, currentColor.b, rangeIndicatorOpacity);
        }

        attackCooldown = 0f;
        originalAttackRate = attackRate;
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

    // --- MUDAN�A #2: L�GICA DE BUSCA ALTERADA PARA USAR TAGS ---
    // Este m�todo agora usa GameObject.FindGameObjectsWithTag, que � a fonte do seu erro.
    // Ao centralizar a l�gica aqui, garantimos que todas as torres usem o valor correto do Inspector.
    private void FindTarget()
    {
        // Esta linha vai nos ajudar a depurar, mostrando qual torre est� procurando qual tag.
        // Debug.Log(gameObject.name + " est� procurando pela tag: '" + enemyTag + "'");

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDistance = Mathf.Infinity;
        Transform newTarget = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            // Verifica se o inimigo est� dentro do alcance E se � o mais pr�ximo at� agora.
            if (distance < closestDistance && distance <= attackRange)
            {
                closestDistance = distance;
                newTarget = enemy.transform;
            }
        }
        currentTarget = newTarget;
    }

    private void HandleRotation()
    {
        if (partToRotate == null || currentTarget == null) return;

        Vector2 direction = currentTarget.position - partToRotate.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        partToRotate.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected abstract void Attack();

    // --- M�TODOS PARA O SISTEMA DE BUFFS ---

    public virtual void ApplyBuff(float rateMultiplier)
    {
        attackRate = originalAttackRate * rateMultiplier;
    }

    // O m�todo foi tornado 'virtual' para permitir que classes filhas o substituam com 'override'.
    public virtual void RemoveBuff()
    {
        attackRate = originalAttackRate;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}