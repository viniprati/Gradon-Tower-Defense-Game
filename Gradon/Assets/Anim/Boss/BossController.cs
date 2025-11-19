using System.Collections.Generic;
using UnityEngine;

// Herda de Enemy para reutilizar a lógica de vida, movimento base e herança de tipo.
public class BossController : Enemy
{
    [Header("Mecânica Especial do Boss")]
    [Tooltip("Quantos hits o boss aguenta antes de se teleportar.")]
    [SerializeField] private int hitsBeforeTeleport = 5;

    // As variáveis de alvo foram separadas para criar a lógica de prioridade.
    [Header("Configuração de Alvo do Boss")]
    [Tooltip("Adicione aqui as tags de TODAS as suas torres (ex: DragonTower, SamuraiTower).")]
    [SerializeField] private List<string> towerTags = new List<string>();

    [Tooltip("A tag exata do seu Totem Principal.")]
    [SerializeField] private string totemTag = "Totem";

    // Contador interno para os hits recebidos.
    private int hitsTaken = 0;

    // A lista de pontos de teleporte, preenchida automaticamente.
    private List<Transform> teleportPoints;

    // A constante para a tag dos pontos de teleporte.
    private const string TELEPORT_POINT_TAG = "TeleportPoint";

    // Sobrescrevemos o Start para configurar tudo automaticamente.
    protected override void Start()
    {
        base.Start(); // Executa a lógica de Start() da classe mãe (Enemy).

        // Encontra os pontos de teleporte.
        teleportPoints = new List<Transform>();
        GameObject[] teleportGOs = GameObject.FindGameObjectsWithTag(TELEPORT_POINT_TAG);
        foreach (var go in teleportGOs)
        {
            teleportPoints.Add(go.transform);
        }
        if (teleportPoints.Count == 0) { Debug.LogError($"[BossController] O Boss não encontrou nenhum GameObject com a tag '{TELEPORT_POINT_TAG}'!"); }

        // Encontra o primeiro alvo seguindo a nova lógica de prioridade.
        FindClosestTarget();
    }

    // Sobrescrevemos o Update para sempre procurar um novo alvo se o atual for destruído.
    protected override void Update()
    {
        if (target == null && !isDead)
        {
            FindClosestTarget();
            if (target == null) return; // Não há mais alvos, o boss para.
        }
        base.Update(); // Executa a lógica de movimento e ataque da classe Enemy.
    }

    // O Boss tem sua própria lógica de ataque para poder danificar tanto torres quanto o totem.
    protected override void PerformAttack()
    {
        if (attackCooldown <= 0f)
        {
            var towerHealth = target.GetComponent<TowerHealth>(); // Ou ConditionalTowerHealth
            if (towerHealth != null)
            {
                towerHealth.TakeDamage(attackDamage);
            }
            else
            {
                var totem = target.GetComponent<Totem>();
                if (totem != null)
                {
                    totem.TakeDamage(attackDamage);
                }
            }
            attackCooldown = 1f / attackRate;
        }
    }

    // Sobrescrevemos a função de tomar dano para adicionar o contador de hits.
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (isDead) return;

        hitsTaken++;
        if (hitsTaken >= hitsBeforeTeleport)
        {
            Teleport();
        }
    }

    // A função de teleporte.
    private void Teleport()
    {
        if (teleportPoints.Count == 0) return;
        hitsTaken = 0;
        int randomIndex = Random.Range(0, teleportPoints.Count);
        transform.position = teleportPoints[randomIndex].position;
        FindClosestTarget();
    }

    // --- FUNÇÃO DE ENCONTRAR ALVO REFEITA COM LÓGICA DE PRIORIDADE ---
    private void FindClosestTarget()
    {
        // 1. PRIMEIRO, PROCURA POR TORRES (PRIORIDADE 1)
        List<GameObject> allTowers = new List<GameObject>();
        foreach (string tag in towerTags)
        {
            allTowers.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }

        // 2. SE ENCONTROU PELO MENOS UMA TORRE...
        if (allTowers.Count > 0)
        {
            Transform closestTower = null;
            float shortestDistance = Mathf.Infinity;
            foreach (var tower in allTowers)
            {
                float distance = Vector2.Distance(transform.position, tower.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestTower = tower.transform;
                }
            }
            target = closestTower;
            // Opcional: descomente a linha abaixo para ver a decisão do Boss no console
            // Debug.Log("<color=orange>Prioridade 1: Atacando a torre mais próxima -> " + target.name + "</color>");
            return; // Sai da função, pois já encontrou o alvo prioritário.
        }

        // 3. SE NÃO HÁ MAIS TORRES, PROCURA PELO TOTEM (PRIORIDADE 2)
        GameObject totemObject = GameObject.FindGameObjectWithTag(totemTag);
        if (totemObject != null)
        {
            target = totemObject.transform;
            // Opcional: descomente a linha abaixo para ver a decisão do Boss no console
            // Debug.Log("<color=red>PRIORIDADE 2: Todas as torres destruídas! Atacando o Totem!</color>");
        }
        else
        {
            // 4. Se não há mais torres nem totem, o Boss venceu.
            target = null;
        }
    }
}