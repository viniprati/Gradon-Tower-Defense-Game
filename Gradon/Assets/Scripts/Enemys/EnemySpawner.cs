// EnemySpawner.cs (Versão Final e Corrigida)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;


// --- LÓGICA PRINCIPAL DO SPAWNER ---
// O nome da classe foi mudado para corresponder ao seu arquivo 'EnemySpawner.cs'
public class EnemySpawner : MonoBehaviour
{
    public static event System.Action OnAllWavesCompleted;

    [Header("Referências da Cena")]
    public Transform[] spawnPoints;

    [Header("Referências da UI")]
    public TextMeshProUGUI waveInfoText;

    [Header("Debug / Teste Rápido")]
    [Tooltip("Arraste um arquivo de fase aqui para testar esta cena diretamente.")]
    public LevelData debugLevel;

    // Variáveis internas
    private int enemiesAlive = 0;

    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Nenhum ponto de spawn configurado!", this.gameObject);
            this.enabled = false;
            return;
        }

        Wave[] wavesToSpawn = null;

        // Lógica para pegar as ondas do GameManager ou do modo de Debug
        if (GameManager.instance != null && GameManager.instance.currentLevelData != null)
        {
            wavesToSpawn = GameManager.instance.currentLevelData.waves;
        }
        else if (debugLevel != null)
        {
            Debug.LogWarning("GameManager não encontrado. Carregando fase de DEBUG: " + debugLevel.name, this.gameObject);
            wavesToSpawn = debugLevel.waves;
        }

        if (wavesToSpawn != null && wavesToSpawn.Length > 0)
        {
            StartCoroutine(SpawnAllWaves(wavesToSpawn));
        }
        else
        {
            Debug.LogError("Nenhuma onda para spawnar! Verifique se a lista 'Waves' está preenchida no seu arquivo LevelData.", this.gameObject);
            this.enabled = false;
        }
    }

    private IEnumerator SpawnAllWaves(Wave[] waves)
    {
        for (int i = 0; i < waves.Length; i++)
        {
            Wave currentWave = waves[i];

            if (waveInfoText != null)
                waveInfoText.text = $"Próxima Onda em {currentWave.delayBeforeWave:F1}s...";

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
            Debug.LogWarning("Tentando spawnar um inimigo, mas o prefab está nulo. Verifique a configuração da onda no seu arquivo de fase.", this.gameObject);
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