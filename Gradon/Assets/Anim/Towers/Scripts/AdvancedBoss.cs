using UnityEngine;

// Garante que AdvancedBoss herde de Enemy
public class AdvancedBoss : Enemy
{
    [Header("Comportamento de Recuo do Boss")]
    [SerializeField] private int attacksBeforeRetreat = 15;

    // Usamos um enum para definir claramente os estados do Boss
    private enum BossState { MovingToTarget, Attacking, Retreating }
    private BossState currentState;

    private Vector3 spawnerPosition;
    private int attackCounter = 0;

    // USAREMOS O OVERRIDE PARA MODIFICAR O COMPORTAMENTO HERDADO
    protected override void Start()
    {
        // 1. Executa a lógica original do Start() da classe Enemy
        // (Isso vai pegar o Rigidbody2D, a vida e o alvo principal)
        base.Start();

        // 2. Adiciona a lógica EXTRA, específica do Boss
        spawnerPosition = transform.position; // Salva a posição inicial
        currentState = BossState.MovingToTarget;
        FindNewTarget(); // Encontra a torre mais próxima, em vez do totem principal
    }

    // SOBRESCREVEMOS COMPLETAMENTE O UPDATE DO ENEMY, POIS O BOSS TEM UMA LÓGICA TOTALMENTE DIFERENTE
    protected override void Update()
    {
        // Ignoramos a verificação de isDead da classe base, pois o Boss pode ter uma lógica diferente
        if (target == null && currentState != BossState.Retreating)
        {
            FindNewTarget();
            if (target == null) return; // Se não há mais alvos, para
        }

        // Máquina de estados do Boss
        switch (currentState)
        {
            case BossState.MovingToTarget:
                MoveToTarget();
                break;
            case BossState.Attacking:
                // Usaremos o PerformAttack herdado, mas com controle de contagem
                AttackTarget();
                break;
            case BossState.Retreating:
                RetreatToSpawner();
                break;
        }
    }

    // Métodos específicos do Boss
    void FindNewTarget()
    {
        // A lógica de encontrar a torre mais próxima
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag("Tower");
        if (allTargets.Length == 0)
        {
            target = null;
            return;
        }

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject targetObject in allTargets)
        {
            float distance = Vector2.Distance(transform.position, targetObject.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = targetObject.transform;
            }
        }

        target = closestTarget; // Define o alvo (target) que a classe Enemy usa
        currentState = BossState.MovingToTarget;
    }

    void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, target.position) <= attackRange)
        {
            currentState = BossState.Attacking;
        }
    }

    void AttackTarget()
    {
        // Reutilizamos a lógica de ataque do Enemy.cs, que já tem cooldown
        PerformAttack();
    }

    // Sobrescrevemos o ataque para adicionar a contagem
    protected override void PerformAttack()
    {
        base.PerformAttack(); // Executa o ataque normal (que já tem cooldown)

        // Verificamos se o ataque realmente aconteceu (se o cooldown permitiu)
        if (attackCooldown <= Time.deltaTime) // Um pequeno truque para saber se o ataque foi executado
        {
            attackCounter++;
            if (attackCounter >= attacksBeforeRetreat)
            {
                currentState = BossState.Retreating;
            }
        }
    }

    void RetreatToSpawner()
    {
        transform.position = Vector2.MoveTowards(transform.position, spawnerPosition, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, spawnerPosition) < 0.1f)
        {
            attackCounter = 0;
            FindNewTarget();
        }
    }
}