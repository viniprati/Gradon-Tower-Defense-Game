using System.Collections.Generic;
using UnityEngine;

// A classe Boss herda de Enemy, ganhando toda a sua funcionalidade base.
public class Boss : Enemy
{
    [Header("Mecânica Especial do Boss")]
    [Tooltip("Arraste todos os seus GameObjects de Spawner/Teleporte para esta lista.")]
    [SerializeField] private List<Transform> teleportPoints = new List<Transform>();

    // Sobrescrevemos o Start para que o Boss encontre seu próprio alvo inicial.
    protected override void Start()
    {
        // Executa a lógica de Start() da classe Enemy (configura vida, etc.)
        base.Start();

        // Em vez de focar no Totem, ele imediatamente procura a torre mais próxima.
        FindClosestTower();
    }

    // Sobrescrevemos o Update para garantir que ele sempre procure um novo alvo se o atual for destruído.
    protected override void Update()
    {
        // Se o alvo atual não existe mais (foi destruído) e o boss não está morto...
        if (target == null && !isDead)
        {
            // ...encontra um novo alvo.
            FindClosestTower();

            // Se mesmo assim não há mais alvos, o boss para (ele venceu).
            if (target == null) return;
        }

        // Depois de garantir que temos um alvo, deixamos a lógica de Update() da classe Enemy
        // (que já cuida de mover e atacar) fazer o seu trabalho.
        base.Update();
    }

    // --- AQUI ESTÁ A LÓGICA PRINCIPAL ---
    // Sobrescrevemos a função de tomar dano para adicionar o teleporte.
    public override void TakeDamage(float damage)
    {
        // 1. Primeiro, executa a lógica original da classe Enemy (perder vida, checar se morreu).
        base.TakeDamage(damage);

        // 2. Se o boss morreu com esse hit, não fazemos mais nada. A função Die() será chamada.
        if (isDead) return;

        // 3. Se ele sobreviveu ao hit, ele se teleporta!
        Teleport();
    }

    private void Teleport()
    {
        // Checagem de segurança para evitar erros se a lista estiver vazia.
        if (teleportPoints == null || teleportPoints.Count == 0)
        {
            Debug.LogError("O Boss não tem pontos de teleporte definidos no Inspector!");
            return;
        }

        Debug.Log("Boss foi atingido e está teleportando!");

        // Escolhe um índice aleatório da lista de pontos de teleporte.
        int randomIndex = Random.Range(0, teleportPoints.Count);

        // Move o boss instantaneamente para a posição do ponto escolhido.
        transform.position = teleportPoints[randomIndex].position;

        // Após se mover, ele precisa recalcular qual é a torre mais próxima de sua nova posição.
        FindClosestTower();
    }

    private void FindClosestTower()
    {
        // Encontra todos os GameObjects na cena com a tag "Tower".
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        if (towers.Length == 0)
        {
            target = null; // Não há mais alvos.
            Debug.Log("Boss venceu! Nenhuma torre restante.");
            return;
        }

        Transform closestTower = null;
        float shortestDistance = Mathf.Infinity;

        // Loop para encontrar a torre mais próxima.
        foreach (var tower in towers)
        {
            float distance = Vector2.Distance(transform.position, tower.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestTower = tower.transform;
            }
        }

        // Define a variável 'target' (que a classe Enemy usa para se mover e atacar) como a torre encontrada.
        target = closestTower;
        Debug.Log("Novo alvo do Boss: " + target.name);
    }
}