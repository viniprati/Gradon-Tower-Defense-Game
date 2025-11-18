using System.Collections.Generic;
using UnityEngine;

public class BossController : Enemy
{
    [Header("Mecânica Especial do Boss")]
    [Tooltip("Arraste todos os seus GameObjects de Spawner/Teleporte para esta lista.")]
    [SerializeField] private List<Transform> teleportPoints = new List<Transform>();

    [Header("Configuração de Alvo do Boss")]
    [Tooltip("Adicione aqui TODAS as tags que o Boss deve considerar como alvos (ex: DragonTower, SamuraiTower, Totem).")]
    [SerializeField] private List<string> targetTags = new List<string>();

    // Sobrescrevemos o Start para o Boss encontrar seu alvo inicial
    protected override void Start()
    {
        base.Start();
        FindClosestTarget();
    }

    // Sobrescrevemos o Update para sempre procurar um novo alvo se o atual for destruído
    protected override void Update()
    {
        if (target == null && !isDead)
        {
            FindClosestTarget();
            if (target == null) return; // Não há mais alvos
        }
        base.Update(); // Executa a lógica de movimento e ataque da classe Enemy
    }

    // Sobrescrevemos a função de tomar dano para adicionar o teleporte
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (isDead) return;
        Teleport();
    }

    private void Teleport()
    {
        if (teleportPoints == null || teleportPoints.Count == 0)
        {
            Debug.LogError("O Boss não tem pontos de teleporte definidos no Inspector!");
            return;
        }
        int randomIndex = Random.Range(0, teleportPoints.Count);
        transform.position = teleportPoints[randomIndex].position;
        FindClosestTarget(); // Procura o novo alvo mais próximo
    }

    // --- ESTA FUNÇÃO FOI COMPLETAMENTE REFEITA ---
    private void FindClosestTarget()
    {
        // 1. Cria uma lista vazia para guardar TODOS os alvos encontrados
        List<GameObject> allPotentialTargets = new List<GameObject>();

        // 2. Loop através de cada tag que definimos no Inspector
        foreach (string tag in targetTags)
        {
            // Encontra todos os objetos com a tag atual
            GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(tag);
            // Adiciona todos eles à nossa lista principal
            allPotentialTargets.AddRange(foundObjects);
        }

        // 3. Se depois de procurar em todas as tags a lista ainda está vazia, não há alvos
        if (allPotentialTargets.Count == 0)
        {
            target = null;
            Debug.LogWarning("Boss não encontrou NENHUM alvo com as tags especificadas. Verifique a lista 'Target Tags' no Inspector e as tags dos objetos na cena.");
            return;
        }

        // 4. Agora, com a lista completa, encontra o mais próximo
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
        Debug.Log("Novo alvo do Boss: " + target.name);
    }
}