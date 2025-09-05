//EnemySpawner.cs 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcadeEnemySpawner : MonoBehaviour
{

    [Header("Referências da Cena")]
    [Tooltip("Arraste todos os objetos que servirão como pontos de spawn para esta lista.")]
    [SerializeField] private List<Transform> spawnPoints;


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

        if (LevelManager.instance != null && LevelManager.instance.currentLevelData != null)
        {
            Wave[] wavesForThisLevel = LevelManager.instance.currentLevelData.waves;

            StartCoroutine(SpawnAllWaves(wavesForThisLevel));
        }
        else
        {
            Debug.LogError("Spawner não conseguiu encontrar os dados da fase no LevelManager! Certifique-se de iniciar o jogo a partir do Menu Principal.", this.gameObject);
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
                SpawnSingleEnemy(group.enemyPrefab);
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }

    private void SpawnSingleEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Tentativa de spawnar um inimigo, mas o prefab é nulo. Verifique a configuração da fase no arquivo LevelData.");
            return;
        }
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}