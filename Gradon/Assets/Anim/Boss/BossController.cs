using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Herda de Enemy para reutilizar a lógica de vida, movimento base e herança de tipo.
public class BossController : Enemy
{
    // --- O "INTERRUPTOR" GLOBAL ---
    public static bool isBossActive { get; private set; }

    [Header("Mecânica Especial do Boss")]
    [Tooltip("Quantos hits o boss aguenta antes de se teleportar.")]
    [SerializeField] private int hitsBeforeTeleport = 5;

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

    // Flag para evitar que a busca por alvo seja chamada múltiplas vezes no mesmo frame.
    private bool isFindingTarget = false;

    private void OnEnable()
    {
        isBossActive = true;
    }

    private void OnDisable()
    {
        isBossActive = false;
    }

    protected override void Start()
    {
        base.Start();

        // Lógica automática para encontrar pontos de teleporte na cena
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

        FindClosestTarget();
    }

    // --- FUNÇÃO UPDATE COM CORROTINA (EVITA PAUSAS) ---
    protected override void Update()
    {
        // Se o alvo foi destruído e não estamos já procurando por um novo...
        if (target == null && !isDead && !isFindingTarget)
        {
            // ...inicia a corrotina para procurar um novo alvo após um pequeno delay (fim do frame).
            StartCoroutine(FindNewTargetAfterDelay());

            // Pausa o comportamento base para este frame para evitar erros de movimento nulo.
            return;
        }

        // Se houver um alvo, executa a lógica de movimento e ataque da classe Enemy.
        base.Update();
    }

    // --- CORROTINA PARA BUSCA DE ALVO ---
    private IEnumerator FindNewTargetAfterDelay()
    {
        isFindingTarget = true;

        // Espera até o final do frame atual. Isso garante que o objeto destruído já saiu da memória.
        yield return new WaitForEndOfFrame();

        // Executa a busca pelo novo alvo.
        FindClosestTarget();

        isFindingTarget = false;
    }
    // ---------------------------------------------

    protected override void PerformAttack()
    {
        if (attackCooldown <= 0f)
        {
            // Verifica se é uma torre (usa o script de vida condicional)
            var towerHealth = target.GetComponent<ConditionalTowerHealth>();
            if (towerHealth != null)
            {
                towerHealth.TakeDamage(attackDamage);
            }
            // Verifica se é o Totem (usa o script de vida normal)
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

    private void Teleport()
    {
        if (teleportPoints.Count == 0) return;

        hitsTaken = 0;
        int randomIndex = Random.Range(0, teleportPoints.Count);
        transform.position = teleportPoints[randomIndex].position;

        // Ao teleportar, recalcula qual é o alvo mais próximo da nova posição
        FindClosestTarget();
    }

    // --- LÓGICA DE PRIORIDADE COM DEDO DURO ---
    private void FindClosestTarget()
    {
        // --- DEDO DURO INICIADO ---
        Debug.Log("--- [DEDO DURO] INICIANDO BUSCA POR ALVOS ---");

        // 1. PRIORIDADE: Coleta todas as torres vivas
        List<GameObject> allTowers = new List<GameObject>();
        foreach (string tag in towerTags)
        {
            GameObject[] found = GameObject.FindGameObjectsWithTag(tag);
            allTowers.AddRange(found);
            // Dedo duro conta quantos achou de cada tipo
            Debug.Log($"[DEDO DURO] Procurando por tag '{tag}': Encontrados {found.Length} objetos.");
        }

        // 2. Se existir ALGUMA torre na cena...
        if (allTowers.Count > 0)
        {
            Transform closestTower = null;
            float shortestDistance = Mathf.Infinity;

            // ...encontra a mais próxima e define como alvo.
            foreach (var tower in allTowers)
            {
                if (tower == null) continue;
                float distance = Vector2.Distance(transform.position, tower.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestTower = tower.transform;
                }
            }
            target = closestTower;

            Debug.Log($"<color=green>[DEDO DURO] ALVO DEFINIDO (TORRE): {target.name}</color>");

            // IMPORTANTE: O 'return' aqui faz ele sair da função e IGNORAR o Totem
            // enquanto houver torres vivas.
            return;
        }

        // 3. Se chegou aqui, é porque a lista de torres estava vazia.
        Debug.Log("[DEDO DURO] Nenhuma torre encontrada. Procurando pelo Totem...");

        GameObject totemObject = GameObject.FindGameObjectWithTag(totemTag);
        if (totemObject != null)
        {
            target = totemObject.transform;
            Debug.Log($"<color=green>[DEDO DURO] ALVO DEFINIDO (TOTEM): {target.name}</color>");
        }
        else
        {
            target = null; // Boss venceu (destruiu tudo).
            Debug.LogError($"<color=red>[DEDO DURO] ALVO NÃO ENCONTRADO!</color> O Boss procurou por torres e pelo Totem ({totemTag}), mas não achou NADA.");
        }
    }
}