using System.Collections.Generic;
using UnityEngine;

// Herda de Enemy para reutilizar a lógica de vida, movimento base e herança de tipo.
public class BossController : Enemy
{
    [Header("Mecânica Especial do Boss")]
    [Tooltip("Quantos hits o boss aguenta antes de se teleportar.")]
    [SerializeField] private int hitsBeforeTeleport = 5;

    [Header("Configuração de Alvo do Boss")]
    [Tooltip("Adicione aqui TODAS as tags que o Boss deve considerar como alvos (ex: DragonTower, SamuraiTower, Totem).")]
    [SerializeField] private List<string> targetTags = new List<string>();

    // Contador interno para os hits recebidos.
    private int hitsTaken = 0;

    // A lista de pontos de teleporte. Agora é privada e será preenchida automaticamente no Start().
    private List<Transform> teleportPoints;

    // A constante para a tag dos pontos de teleporte.
    private const string TELEPORT_POINT_TAG = "TeleportPoint";

    // Sobrescrevemos o Start para configurar tudo automaticamente.
    protected override void Start()
    {
        // Executa a lógica de Start() da classe mãe (Enemy), que define a vida.
        base.Start();

        // Encontra todos os GameObjects na cena com a tag "TeleportPoint".
        teleportPoints = new List<Transform>();
        GameObject[] teleportGOs = GameObject.FindGameObjectsWithTag(TELEPORT_POINT_TAG);
        foreach (var go in teleportGOs)
        {
            teleportPoints.Add(go.transform);
        }

        if (teleportPoints.Count == 0)
        {
            Debug.LogError($"[BossController] O Boss não encontrou nenhum GameObject com a tag '{TELEPORT_POINT_TAG}'!");
        }

        // Encontra o primeiro alvo para atacar, ignorando o alvo padrão da classe Enemy.
        FindClosestTarget();
    }

    // Sobrescrevemos o Update para garantir que ele sempre procure um novo alvo se o atual for destruído.
    protected override void Update()
    {
        if (target == null && !isDead)
        {
            FindClosestTarget();
            if (target == null) return; // Não há mais alvos, o boss para.
        }

        // Executa a lógica normal de movimento e checagem de alcance da classe Enemy.
        base.Update();
    }

    // --- LÓGICA DE ATAQUE EXCLUSIVA DO BOSS ---
    // O Boss sobrescreve a lógica de ataque padrão da classe Enemy.
    protected override void PerformAttack()
    {
        if (attackCooldown <= 0f)
        {
            // 1. Tenta encontrar um script TowerHealth (para torres normais).
            var towerHealth = target.GetComponent<TowerHealth>();
            if (towerHealth != null)
            {
                towerHealth.TakeDamage(attackDamage);
            }
            // 2. Se não encontrar, verifica se o alvo é o Totem.
            else
            {
                var totem = target.GetComponent<Totem>();
                if (totem != null)
                {
                    totem.TakeDamage(attackDamage);
                }
            }

            // Reseta o cooldown do ataque.
            attackCooldown = 1f / attackRate;
        }
    }

    // Sobrescrevemos a função de tomar dano para adicionar o contador de hits e o teleporte.
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage); // Aplica o dano e checa se morreu.
        if (isDead) return;

        hitsTaken++; // Incrementa o contador de hits.

        if (hitsTaken >= hitsBeforeTeleport)
        {
            Teleport(); // Se atingiu o limite, teleporta.
        }
    }

    // A função de teleporte.
    private void Teleport()
    {
        if (teleportPoints.Count == 0) return; // Checagem de segurança.

        hitsTaken = 0; // Reseta o contador para o próximo ciclo.
        int randomIndex = Random.Range(0, teleportPoints.Count);
        transform.position = teleportPoints[randomIndex].position;

        // Após teleportar, procura o novo alvo mais próximo.
        FindClosestTarget();
    }

    // A função para encontrar o alvo mais próximo entre todas as tags especificadas.
    private void FindClosestTarget()
    {
        List<GameObject> allPotentialTargets = new List<GameObject>();
        foreach (string tag in targetTags)
        {
            allPotentialTargets.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }

        if (allPotentialTargets.Count == 0)
        {
            target = null;
            return;
        }

        Transform closestTarget = null;
        float shortestDistance = Mathf.Infinity;
        foreach (var potentialTarget in allPotentialTargets)
        {
            if (potentialTarget == null) continue;

            float distance = Vector2.Distance(transform.position, potentialTarget.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestTarget = potentialTarget.transform;
            }
        }
        target = closestTarget;
    }
}