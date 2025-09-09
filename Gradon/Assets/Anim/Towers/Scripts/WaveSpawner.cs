// WaveSpawner.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    public static event System.Action OnAllWavesCompleted; // Evento para anunciar a vitória

    [Header("Referências da Cena")]
    public Transform[] spawnPoints;

    private Wave[] waves;
    private int enemiesAlive = 0;
    private int totalWaves;
    private int wavesCompleted = 0;

    void Start()
    {
        if (GameManager.instance != null && GameManager.instance.currentLevelData != null)
        {
            this.waves = GameManager.instance.currentLevelData.waves;
            this.totalWaves = waves.Length;
            StartCoroutine(SpawnAllWaves());
        }
        else
        {
            Debug.LogError("WaveSpawner não encontrou dados da fase! Inicie pelo menu.");
            this.enabled = false;
        }
    }

    private IEnumerator SpawnAllWaves()
    {
        foreach (Wave wave in waves)
        {
            yield return new WaitForSeconds(wave.delayBeforeWave);
            yield return StartCoroutine(SpawnWave(wave));

            // Espera todos os inimigos da onda serem derrotados antes de prosseguir
            while (enemiesAlive > 0)
            {
                yield return null;
            }

            wavesCompleted++;
        }

        // Se todas as ondas foram concluídas e todos inimigos morreram
        if (wavesCompleted == totalWaves && enemiesAlive == 0)
        {
            OnAllWavesCompleted?.Invoke(); // Dispara o evento de vitória
            GameManager.instance.WinLevel();
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        foreach (EnemyGroup group in wave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(group.enemyPrefab);
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyGO = Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);

        Enemy enemyScript = enemyGO.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.OnDeath += OnEnemyKilled; // Registra o evento de morte
        }

        enemiesAlive++;
    }

    // Chamado quando um inimigo morre
    private void OnEnemyKilled(Enemy enemy)
    {
        enemiesAlive--;
        enemy.OnDeath -= OnEnemyKilled; // Desregistra o evento para evitar memory leaks
    }
}