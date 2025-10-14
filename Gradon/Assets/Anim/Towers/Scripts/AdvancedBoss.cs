using UnityEngine;

public class AdvancedBoss : Enemy
{
    // Usamos um enum para definir claramente os estados do Boss
    private enum BossState { MovingToTarget, Attacking, Retreating }
    private BossState currentState;

    [Header("Atributos do Boss")]
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackRate = 0.5f; // Ataques por segundo

    [Header("Comportamento de Recuo")]
    [SerializeField] private int attacksBeforeRetreat = 15;

    private Transform currentTarget;
    private Vector3 spawnerPosition;
    private int attackCounter = 0;
    private float attackCooldown = 0f;

    void Start()
    {
        // Salva a posi��o inicial como o ponto de recuo (spawner)
        spawnerPosition = transform.position;
        FindNewTarget();
    }

    void Update()
    {
        if (currentTarget == null && currentState != BossState.Retreating)
        {
            // Se o alvo foi destru�do, encontre um novo
            FindNewTarget();
            // Se mesmo assim n�o achar, n�o faz nada
            if (currentTarget == null) return;
        }

        // M�quina de estados: executa a l�gica baseada no estado atual
        switch (currentState)
        {
            case BossState.MovingToTarget:
                MoveToTarget();
                break;
            case BossState.Attacking:
                AttackTarget();
                break;
            case BossState.Retreating:
                RetreatToSpawner();
                break;
        }
    }

    void FindNewTarget()
    {
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag("Tower");
        if (allTargets.Length == 0)
        {
            Debug.Log("Boss n�o encontrou alvos restantes.");
            currentTarget = null;
            return;
        }

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        // Encontra a torre mais pr�xima
        foreach (GameObject targetObject in allTargets)
        {
            float distance = Vector2.Distance(transform.position, targetObject.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = targetObject.transform;
            }
        }

        currentTarget = closestTarget;
        currentState = BossState.MovingToTarget;
        Debug.Log("Novo alvo do Boss: " + currentTarget.name);
    }

    void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, currentTarget.position) <= attackRange)
        {
            currentState = BossState.Attacking; // Chegou perto o suficiente para atacar
        }
    }

    void AttackTarget()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
            return;
        }

        // Ataca
        TowerHealth targetHealth = currentTarget.GetComponent<TowerHealth>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage);
        }
        attackCooldown = 1f / attackRate;
        attackCounter++;
        Debug.Log("Boss atacou! Contagem: " + attackCounter);

        // Verifica se � hora de recuar
        if (attackCounter >= attacksBeforeRetreat)
        {
            currentState = BossState.Retreating;
            Debug.Log("Boss vai recuar!");
        }
    }

    void RetreatToSpawner()
    {
        transform.position = Vector2.MoveTowards(transform.position, spawnerPosition, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, spawnerPosition) < 0.1f)
        {
            // Chegou no spawner, reseta e come�a de novo
            Debug.Log("Boss recuou. Come�ando novo ciclo.");
            attackCounter = 0;
            FindNewTarget();
        }
    }
}