using System.Collections.Generic;
using UnityEngine;

// O nome da classe agora é BossController, e ela herda de Enemy.
public class BossController : Enemy
{
    [Header("Mecânica Especial do Boss")]
    [Tooltip("Arraste todos os seus GameObjects de Spawner/Teleporte para esta lista.")]
    [SerializeField] private List<Transform> teleportPoints = new List<Transform>();

    // Sobrescrevemos o Start para que o Boss encontre seu próprio alvo inicial.
    protected override void Start()
    {
        base.Start(); // Executa a lógica de Start() da classe Enemy
        FindClosestTower(); // Encontra a torre mais próxima como seu primeiro alvo
    }

    // Sobrescrevemos o Update para sempre procurar um novo alvo se o atual for destruído.
    protected override void Update()
    {
        if (target == null && !isDead)
        {
            FindClosestTower();
            if (target == null) return;
        }
        base.Update(); // Executa a lógica normal de movimento e ataque do Enemy
    }

    // Sobrescrevemos a função de tomar dano para adicionar o teleporte.
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage); // Aplica o dano
        if (isDead) return; // Se morreu, para
        Teleport(); // Se sobreviveu, teleporta!
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
        FindClosestTower(); // Procura o novo alvo mais próximo
    }

    private void FindClosestTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
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
        target = closestTower;
    }
}