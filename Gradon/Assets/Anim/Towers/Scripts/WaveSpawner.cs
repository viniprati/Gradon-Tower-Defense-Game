// WaveSpawner.cs

using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [Header("Referências da Cena")]
    public Transform[] spawnPoints;

    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("ERRO: Nenhum ponto de spawn foi configurado no WaveSpawner! Arraste os objetos para a lista no Inspector.", this.gameObject);
            this.enabled = false;
            return;
        }

        if (LevelManager.instance != null && LevelManager.instance.currentLevelData != null)
        {
            Wave[] wavesForThisLevel = LevelManager.instance.currentLevelData.waves;
            StartCoroutine(SpawnAllWaves(wavesForThisLevel));
        }
        else
        {
            Debug.LogError("WaveSpawner não conseguiu encontrar os dados da fase no LevelManager! Certifique-se de iniciar o jogo a partir do Menu Principal.", this.gameObject);
            this.enabled = false;
        }
    }

    private IEnumerator SpawnAllWaves(Wave[] waves)
    {
        for (int i = 0; i < waves.Length; i++)
        {
            Wave currentWave = waves[i];

            yield return new WaitForSeconds(currentWave.delayBeforeWave);

            Debug.Log($"<color=orange>Iniciando Onda {i + 1}: {currentWave.waveName}</color>");

            yield return StartCoroutine(SpawnCurrentWave(currentWave));
        }

        Debug.Log("<color=green>FASE CONCLUÍDA! Todas as ondas foram derrotadas!</color>");
    }

    private IEnumerator SpawnCurrentWave(Wave wave)
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
            Debug.LogWarning("Tentativa de spawnar um inimigo, mas o prefab é nulo. Verifique a configuração da fase no arquivo LevelData.");
            return;
        }

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
    }
}