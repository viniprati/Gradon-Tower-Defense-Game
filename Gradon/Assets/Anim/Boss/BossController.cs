using System.Collections.Generic;
using System.Linq; // Importante: necessário para usar .ToList()
using UnityEngine;

public class BossController : Enemy
{
    // A lista de teleporte agora é privada. Ela será preenchida automaticamente.
    private List<Transform> teleportPoints;

    [Header("Configuração de Alvo do Boss")]
    [Tooltip("Adicione aqui TODAS as tags que o Boss deve considerar como alvos.")]
    [SerializeField] private List<string> targetTags = new List<string>();

    // A tag que usaremos para encontrar os pontos de teleporte
    private const string TELEPORT_POINT_TAG = "TeleportPoint";

    // Sobrescrevemos o Start para configurar tudo automaticamente
    protected override void Start()
    {
        base.Start();

        // --- NOVA LÓGICA AUTOMÁTICA ---
        // 1. Encontra todos os GameObjects com a tag "TeleportPoint"
        GameObject[] teleportObjects = GameObject.FindGameObjectsWithTag(TELEPORT_POINT_TAG);

        // 2. Transforma a lista de GameObjects em uma lista de Transforms
        teleportPoints = new List<Transform>();
        foreach (GameObject go in teleportObjects)
        {
            teleportPoints.Add(go.transform);
        }

        // 3. Checagem de segurança e log de diagnóstico
        if (teleportPoints.Count == 0)
        {
            Debug.LogError($"O Boss não encontrou nenhum GameObject com a tag '{TELEPORT_POINT_TAG}'! A mecânica de teleporte não funcionará.");
        }
        else
        {
            Debug.Log($"Boss encontrou {teleportPoints.Count} pontos de teleporte.");
        }
        // ---------------------------------

        // A lógica de encontrar o primeiro alvo continua a mesma
        FindClosestTarget();
    }

    // O resto do script permanece o mesmo...

    protected override void Update()
    {
        if (target == null && !isDead)
        {
            FindClosestTarget();
            if (target == null) return;
        }
        base.Update();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (isDead) return;
        Teleport();
    }

    private void Teleport()
    {
        // A checagem de segurança agora é ainda mais importante
        if (teleportPoints == null || teleportPoints.Count == 0)
        {
            // Se não encontrou nenhum ponto no Start, ele não tenta teleportar.
            return;
        }

        int randomIndex = Random.Range(0, teleportPoints.Count);
        transform.position = teleportPoints[randomIndex].position;
        FindClosestTarget();
    }

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