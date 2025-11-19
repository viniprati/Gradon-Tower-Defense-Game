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

    // Flag para evitar que a busca por alvo seja chamada múltiplas vezes
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

    // --- FUNÇÃO UPDATE CORRIGIDA COM CORROTINA ---
    protected override void Update()
    {
        // Se o alvo foi destruído e não estamos já procurando por um novo...
        if (target == null && !isDead && !isFindingTarget)
        {
            // ...inicia a corrotina para procurar um novo alvo após um pequeno delay.
            StartCoroutine(FindNewTargetAfterDelay());

            // Pausa o comportamento base para este frame para evitar erros.
            return;
        }

        // Se houver um alvo, executa a lógica de movimento e ataque da classe Enemy.
        base.Update();
    }

    // --- NOVA CORROTINA PARA BUSCA DE ALVO ---
    private IEnumerator FindNewTargetAfterDelay()
    {
        isFindingTarget = true; // Sinaliza que estamos no processo de busca

        // Espera até o final do frame atual. Neste ponto, a torre destruída já terá sido removida da cena.
        yield return new WaitForEndOfFrame();

        // Executa a busca pelo novo alvo.
        FindClosestTarget();

        isFindingTarget = false; // Sinaliza que terminamos a busca
    }
    // ---------------------------------------------

    protected override void PerformAttack()
    {
        if (attackCooldown <= 0f)
        {
            var towerHealth = target.GetComponent<ConditionalTowerHealth>();
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
        FindClosestTarget();
    }

    private void FindClosestTarget()
    {
        // Lógica de prioridade 1: Torres
        List<GameObject> allTowers = new List<GameObject>();
        foreach (string tag in towerTags)
        {
            allTowers.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }

        if (allTowers.Count > 0)
        {
            Transform closestTower = null;
            float shortestDistance = Mathf.Infinity;
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
            return;
        }

        // Lógica de prioridade 2: Totem
        GameObject totemObject = GameObject.FindGameObjectWithTag(totemTag);
        if (totemObject != null)
        {
            target = totemObject.transform;
        }
        else
        {
            target = null;
        }
    }
}