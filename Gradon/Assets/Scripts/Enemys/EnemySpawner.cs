// WaveSpawner.cs

using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Refer�ncias da Cena")]
    [Tooltip("Arraste todos os objetos que servir�o como pontos de spawn para esta lista.")]
    public Transform[] spawnPoints;

    [Header("Debug / Teste R�pido")]
    [Tooltip("Se o jogo for iniciado diretamente nesta cena, esta fase ser� carregada para teste. Deixe em branco para o funcionamento normal.")]
    public LevelData debugLevel; // Campo para arrastar uma fase padr�o para testes

    /// <summary>
    /// Chamado pela Unity quando a cena de jogo come�a.
    /// </summary>
    void Start()
    {
        // Checagem de seguran�a inicial
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("ERRO: Nenhum ponto de spawn foi configurado no WaveSpawner! Desativando o spawner.", this.gameObject);
            this.enabled = false;
            return;
        }

        Wave[] wavesToSpawn = null;

        // Tenta pegar os dados do LevelManager (o m�todo principal)
        if (LevelManager.instance != null && LevelManager.instance.currentLevelData != null)
        {
            wavesToSpawn = LevelManager.instance.currentLevelData.waves;
        }
        // Se o LevelManager n�o tiver dados, usa a fase de debug como um "plano B"
        else if (debugLevel != null)
        {
            Debug.LogWarning("LevelManager n�o encontrado ou sem dados de fase. Carregando fase de DEBUG: " + debugLevel.name, this.gameObject);
            wavesToSpawn = debugLevel.waves;
        }

        // Verifica se temos alguma onda para spawnar (seja do LevelManager ou do modo de debug)
        if (wavesToSpawn != null && wavesToSpawn.Length > 0)
        {
            // Se sim, inicia a rotina de spawn
            StartCoroutine(SpawnAllWaves(wavesToSpawn));
        }
        else
        {
            // Se n�o, exibe um erro claro e desativa o spawner
            Debug.LogError("Nenhuma onda para spawnar! Verifique se o LevelManager est� funcionando ou se a fase de Debug foi configurada.", this.gameObject);
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

        Debug.Log("<color=green>FASE CONCLU�DA! Todas as ondas foram derrotadas!</color>");
        // Adicione aqui a l�gica de vit�ria
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
            Debug.LogWarning("Tentativa de spawnar um inimigo, mas o prefab � nulo. Verifique a configura��o da fase no arquivo LevelData.");
            return;
        }

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
    }
}