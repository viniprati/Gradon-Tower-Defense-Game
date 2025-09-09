// WaveSpawner.cs (Vers�o Aut�noma e Corrigida)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// --- DEFINI��ES DAS ONDAS (VIVEM AQUI) ---
[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnInterval;
}

[System.Serializable]
public class Wave
{
    public string waveName;
    public EnemyGroup[] enemyGroups;
    public float delayBeforeWave;
}


// --- L�GICA PRINCIPAL DO SPAWNER ---
public class WaveSpawner : MonoBehaviour
{
    public static event System.Action OnAllWavesCompleted;

    [Header("Configura��o das Ondas")]
    [Tooltip("Configure aqui todas as ondas para esta fase.")]
    public Wave[] waves;

    [Header("Refer�ncias da Cena")]
    public Transform[] spawnPoints;

    [Header("Refer�ncias da UI")]
    public TextMeshProUGUI waveInfoText;

    private int enemiesAlive = 0;

    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Nenhum ponto de spawn configurado no WaveSpawner!", this.gameObject);
            this.enabled = false;
            return;
        }

        if (waves.Length == 0)
        {
            Debug.LogError("Nenhuma onda configurada no WaveSpawner! Preencha a lista 'Waves' no Inspector.", this.gameObject);
            this.enabled = false;
            return;
        }

        StartCoroutine(SpawnAllWaves());
    }

    private IEnumerator SpawnAllWaves()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            Wave currentWave = waves[i];

            if (waveInfoText != null)
                waveInfoText.text = $"Pr�xima Onda em {currentWave.delayBeforeWave:F1}s...";

            yield return new WaitForSeconds(currentWave.delayBeforeWave);

            if (waveInfoText != null)
                waveInfoText.text = $"Onda {i + 1} / {waves.Length}";

            yield return StartCoroutine(SpawnWave(currentWave));

            while (enemiesAlive > 0)
            {
                yield return null;
            }
        }

        OnAllWavesCompleted?.Invoke();

        if (GameManager.instance != null)
        {
            GameManager.instance.HandleGameOver(true);
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
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Tentando spawnar um inimigo, mas o prefab est� nulo. Verifique a configura��o da onda no Inspector.", this.gameObject);
            return;
        }

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyGO = Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);

        Enemy enemyScript = enemyGO.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.OnDeath += OnEnemyKilled;
        }
        enemiesAlive++;
    }

    private void OnEnemyKilled(Enemy enemy)
    {
        enemiesAlive--;
        if (enemy != null)
        {
            enemy.OnDeath -= OnEnemyKilled;
        }
    }
}