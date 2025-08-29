//EnemySpawner.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Este script vai funcionar perfeitamente, pois ele usará a definição de 'EnemyProgression'
// que está no seu outro arquivo, 'EnemyProgression.cs'.
public class ArcadeEnemySpawner : MonoBehaviour
{
    [Header("Progressão de Inimigos")]
    [Tooltip("Configure a lista de inimigos e quando eles devem começar a aparecer.")]
    [SerializeField] private List<EnemyProgression> enemyProgression;

    [Header("Pontos de Spawn")]
    [Tooltip("Se esta lista estiver vazia, o spawner tentará usar todos os objetos filhos como pontos de spawn.")]
    [SerializeField] private List<Transform> spawnPoints;

    [Header("Configuração de Dificuldade")]
    [SerializeField] private float initialSpawnInterval = 2.5f;
    [SerializeField] private float minSpawnInterval = 0.4f;
    [SerializeField] private float timeToIncreaseDifficulty = 15f;
    [SerializeField] private float intervalReduction = 0.1f;
    [SerializeField] private float timeToIncreaseBurstCount = 45f;
    [SerializeField] private int maxEnemiesInBurst = 4;

    private float gameTime = 0f;
    private float currentSpawnInterval;
    private int enemiesPerBurst = 1;
    private List<GameObject> availableEnemies = new List<GameObject>();

    void Awake()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            spawnPoints = new List<Transform>();
            foreach (Transform child in transform)
            {
                spawnPoints.Add(child);
            }
        }
    }

    void Start()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("ERRO: Nenhum ponto de spawn foi configurado para o '" + gameObject.name + "'. Desativando o spawner.", this.gameObject);
            this.enabled = false;
            return;
        }

        currentSpawnInterval = initialSpawnInterval;
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        CheckForNewEnemyUnlocks();
        HandleDifficultyScaling();
    }

    private void CheckForNewEnemyUnlocks()
    {
        foreach (var progressionEntry in enemyProgression)
        {
            if (progressionEntry.enemyPrefab != null && gameTime >= progressionEntry.timeToStartSpawning && !availableEnemies.Contains(progressionEntry.enemyPrefab))
            {
                availableEnemies.Add(progressionEntry.enemyPrefab);
            }
        }
    }

    private void HandleDifficultyScaling()
    {
        int difficultySteps = Mathf.FloorToInt(gameTime / timeToIncreaseDifficulty);
        currentSpawnInterval = initialSpawnInterval - (difficultySteps * intervalReduction);
        currentSpawnInterval = Mathf.Max(currentSpawnInterval, minSpawnInterval);

        int burstSteps = Mathf.FloorToInt(gameTime / timeToIncreaseBurstCount);
        enemiesPerBurst = Mathf.Min(1 + burstSteps, maxEnemiesInBurst);
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnInterval);
            if (availableEnemies.Count > 0 && spawnPoints.Count > 0)
            {
                for (int i = 0; i < enemiesPerBurst; i++)
                {
                    SpawnSingleEnemy();
                    if (enemiesPerBurst > 1) yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }

    private void SpawnSingleEnemy()
    {
        GameObject enemyToSpawn = availableEnemies[Random.Range(0, availableEnemies.Count)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
    }
}
// NÃO HÁ NADA AQUI EMBAIXO! A definição duplicada foi removida.