using System.Collections.Generic;
using UnityEngine;

public class TeleportingBoss : Enemy
{
    [Header("Mecânica de Teleporte do Boss")]
    [Tooltip("Quantos hits o boss aguenta antes de se teleportar.")]
    [SerializeField] private int hitsToTeleport = 5;

    [Tooltip("Arraste todos os seus GameObjects de Spawner para esta lista.")]
    [SerializeField] private List<Transform> spawnerPositions = new List<Transform>();

    private int hitsTaken = 0;

    // Sobrescrevemos o Start para que o Boss procure o alvo certo
    protected override void Start()
    {
        base.Start(); // Executa o Start() da classe Enemy
        FindClosestTower(); // E então encontra seu próprio alvo
    }

    // Sobrescrevemos o Update para garantir que ele sempre tenha um alvo
    protected override void Update()
    {
        if (target == null && !isDead)
        {
            FindClosestTower();
            if (target == null) return; // Não há mais torres, Boss venceu
        }

        base.Update(); // Executa a lógica normal de movimento e ataque do Enemy
    }

    // AQUI ESTÁ A LÓGICA PRINCIPAL: Sobrescrevemos a função de tomar dano
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage); // Primeiro, aplica o dano e checa se morreu

        if (isDead) return; // Se morreu, não faz mais nada

        hitsTaken++;
        Debug.Log("Boss tomou um hit! Contagem: " + hitsTaken);

        if (hitsTaken >= hitsToTeleport)
        {
            TeleportToRandomSpawner();
        }
    }

    private void TeleportToRandomSpawner()
    {
        if (spawnerPositions.Count == 0)
        {
            Debug.LogError("A lista de spawners do Boss está vazia!");
            return;
        }

        Debug.Log("Boss teleportando!");
        hitsTaken = 0; // Reseta o contador

        int randomIndex = Random.Range(0, spawnerPositions.Count);
        transform.position = spawnerPositions[randomIndex].position;

        // Após teleportar, procura a torre mais próxima de sua nova localização
        FindClosestTower();
    }

    private void FindClosestTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower"); // Certifique-se que suas torres têm a tag "Tower"

        if (towers.Length == 0)
        {
            target = null;
            return;
        }

        Transform closestTower = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var tower in towers)
        {
            float distance = Vector2.Distance(transform.position, tower.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestTower = tower.transform;
            }
        }

        target = closestTower; // Define o alvo da classe Enemy como a torre encontrada
    }
}