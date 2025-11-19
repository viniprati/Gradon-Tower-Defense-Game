using System.Collections.Generic;
using UnityEngine;

// Herda de Enemy para reutilizar a lógica de vida, movimento base e herança de tipo.
public class BossController : Enemy
{
    // --- O "INTERRUPTOR" GLOBAL ---
    // 'static' significa que esta variável pertence à classe e pode ser acessada de qualquer lugar.
    // Qualquer script pode verificar se o Boss está ativo com: if (BossController.isBossActive)
    public static bool isBossActive { get; private set; }
    // ---------------------------------

    [Header("Mecânica Especial do Boss")]
    [Tooltip("Quantos hits o boss aguenta antes de se teleportar.")]
    [SerializeField] private int hitsBeforeTeleport = 5;

    [Header("Configuração de Alvo do Boss")]
    [Tooltip("Adicione aqui TODAS as tags que o Boss deve considerar como alvos.")]
    [SerializeField] private List<string> targetTags = new List<string>();

    // Contador interno para os hits recebidos.
    private int hitsTaken = 0;

    // A lista de pontos de teleporte, preenchida automaticamente.
    private List<Transform> teleportPoints;

    // A constante para a tag dos pontos de teleporte.
    private const string TELEPORT_POINT_TAG = "TeleportPoint";

    // OnEnable é chamado sempre que o Boss é ativado ou criado (spawnado).
    private void OnEnable()
    {
        // Liga o interruptor, tornando as torres vulneráveis.
        isBossActive = true;
        Debug.Log("<color=red>O BOSS ENTROU EM CAMPO! Torres estão vulneráveis!</color>");
    }

    // OnDisable é chamado quando o Boss é desativado ou destruído.
    private void OnDisable()
    {
        // Desliga o interruptor, tornando as torres invulneráveis novamente.
        isBossActive = false;
        Debug.Log("<color=green>O Boss foi derrotado! Torres estão seguras.</color>");
    }

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

        if (teleportPoints.Count == 0)
        {
            Debug.LogError($"[BossController] O Boss não encontrou nenhum GameObject com a tag '{TELEPORT_POINT_TAG}'!");
        }

        // Encontra o primeiro alvo.
        FindClosestTarget();
    }

    // Sobrescrevemos o Update para sempre procurar um novo alvo.
    protected override void Update()
    {
        if (target == null && !isDead)
        {
            FindClosestTarget();
            if (target == null) return;
        }
        base.Update();
    }

    // --- LÓGICA DE ATAQUE ATUALIZADA ---
    // O Boss agora procura pelo novo script de vida 'ConditionalTowerHealth'.
    protected override void PerformAttack()
    {
        if (attackCooldown <= 0f)
        {
            // Tenta atacar uma torre com vida condicional.
            var towerHealth = target.GetComponent<ConditionalTowerHealth>();
            if (towerHealth != null)
            {
                towerHealth.TakeDamage(attackDamage);
            }
            // Se não for uma torre, verifica se é o Totem (que está sempre vulnerável).
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

    // A função para encontrar o alvo mais próximo.
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