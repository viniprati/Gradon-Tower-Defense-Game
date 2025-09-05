// WaveSpawner.cs

using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Referências da Cena")]
    [Tooltip("Arraste todos os objetos que servirão como pontos de spawn para esta lista.")]
    public Transform[] spawnPoints;

    [Header("Debug / Teste Rápido")]
    [Tooltip("Se o jogo for iniciado diretamente nesta cena, esta fase será carregada para teste. Deixe em branco para o funcionamento normal.")]
    public LevelData debugLevel; // Campo para arrastar uma fase padrão para testes

    /// <summary>
    /// Chamado pela Unity quando a cena de jogo começa.
    /// </summary>
    void Start()
    {
        // Checagem de segurança inicial
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("ERRO: Nenhum ponto de spawn foi configurado no WaveSpawner! Desativando o spawner.", this.gameObject);
            this.enabled = false;
            return;
        }

        Wave[] wavesToSpawn = null;

        // Tenta pegar os dados do LevelManager (o método principal)
        if (LevelManager.instance != null && LevelManager.instance.currentLevelData != null)
        {
            wavesToSpawn = LevelManager.instance.currentLevelData.waves;
        }
        // Se o LevelManager não tiver dados, usa a fase de debug como um "plano B"
        else if (debugLevel != null)
        {
            Debug.LogWarning("LevelManager não encontrado ou sem dados de fase. Carregando fase de DEBUG: " + debugLevel.name, this.gameObject);
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
            // Se não, exibe um erro claro e desativa o spawner
            Debug.LogError("Nenhuma onda para spawnar! Verifique se o LevelManager está funcionando ou se a fase de Debug foi configurada.", this.gameObject);
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
        // Adicione aqui a lógica de vitória
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