// WaveSpawner.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class WaveSpawner : MonoBehaviour
{
    [Header("Configuração das Ondas")]
    [Tooltip("Crie e configure todas as ondas para esta fase aqui no Inspector.")]
    public Wave[] waves; 

    [Header("Referências")]
    [Tooltip("Arraste todos os objetos que servirão como pontos de spawn para esta lista.")]
    public Transform[] spawnPoints;


    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("ERRO: Nenhum ponto de spawn foi configurado no WaveSpawner! Arraste os objetos para a lista no Inspector.", this.gameObject);
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
            Debug.LogWarning("Tentativa de spawnar um inimigo, mas o prefab é nulo. Verifique a configuração das ondas no Inspector.");
            return;
        }

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
    }
}