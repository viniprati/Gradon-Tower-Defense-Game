// WaveSpawner.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Essencial para usar o Dicion�rio

public class WaveSpawner : MonoBehaviour
{
    [Header("Dicion�rio de Inimigos")]
    [Tooltip("ARRASTE AQUI TODOS OS PREFABS de inimigos que o jogo pode usar.")]
    public List<GameObject> enemyPrefabsLibrary; // O lugar para arrastar todos os seus prefabs

    [Header("Refer�ncias da Cena")]
    [Tooltip("Arraste todos os objetos que servir�o como pontos de spawn para esta lista.")]
    public Transform[] spawnPoints;

    [Header("Debug / Teste R�pido")]
    [Tooltip("Se o jogo for iniciado diretamente nesta cena, esta fase ser� carregada para teste.")]
    public LevelData debugLevel;

    // Dicion�rio interno para busca r�pida de prefabs pelo nome.
    private Dictionary<string, GameObject> enemyDictionary;

    /// <summary>
    /// Awake � chamado antes de Start. Ideal para inicializar estruturas de dados.
    /// </summary>
    void Awake()
    {
        // Cria o dicion�rio para uma busca super r�pida de inimigos pelo nome.
        enemyDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in enemyPrefabsLibrary)
        {
            if (prefab != null && !enemyDictionary.ContainsKey(prefab.name))
            {
                enemyDictionary.Add(prefab.name, prefab);
            }
            else
            {
                Debug.LogWarning($"Prefab de inimigo duplicado ou nulo encontrado na biblioteca: '{prefab?.name}'. Ignorando.", this.gameObject);
            }
        }
    }

    /// <summary>
    /// Chamado quando a cena come�a.
    /// </summary>
    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("ERRO: Nenhum ponto de spawn foi configurado!", this.gameObject);
            this.enabled = false;
            return;
        }

        Wave[] wavesToSpawn = null;

        // Tenta pegar os dados da fase do LevelManager (modo normal de jogo)
        if (LevelManager.instance != null && LevelManager.instance.currentLevelData != null)
        {
            wavesToSpawn = LevelManager.instance.currentLevelData.waves;
        }
        // Se falhar, usa a fase de debug (modo de teste)
        else if (debugLevel != null)
        {
            Debug.LogWarning("LevelManager n�o encontrado. Carregando fase de DEBUG: " + debugLevel.name, this.gameObject);
            wavesToSpawn = debugLevel.waves;
        }

        if (wavesToSpawn != null && wavesToSpawn.Length > 0)
        {
            StartCoroutine(SpawnAllWaves(wavesToSpawn));
        }
        else
        {
            Debug.LogError("Nenhuma onda para spawnar! Verifique o LevelManager ou a fase de Debug.", this.gameObject);
            this.enabled = false;
        }
    }

    private IEnumerator SpawnAllWaves(Wave[] waves)
    {
        foreach (Wave wave in waves)
        {
            yield return new WaitForSeconds(wave.delayBeforeWave);
            Debug.Log($"<color=orange>Iniciando Onda: {wave.waveName}</color>");
            yield return StartCoroutine(SpawnCurrentWave(wave));
        }
        Debug.Log("<color=green>FASE CONCLU�DA!</color>");
    }

    private IEnumerator SpawnCurrentWave(Wave wave)
    {
        foreach (EnemyGroup group in wave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemyByName(group.enemyType);
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }

    /// <summary>
    /// Spawna um inimigo procurando seu prefab no dicion�rio pelo nome.
    /// </summary>
    private void SpawnEnemyByName(string enemyTypeName)
    {
        // Procura o prefab no nosso dicion�rio usando o nome fornecido no LevelData.
        if (enemyDictionary.TryGetValue(enemyTypeName, out GameObject prefabToSpawn))
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(prefabToSpawn, randomSpawnPoint.position, randomSpawnPoint.rotation);
        }
        else
        {
            // Erro claro se voc� digitou um nome errado no LevelData.
            Debug.LogError($"O tipo de inimigo '{enemyTypeName}' n�o foi encontrado no Dicion�rio! Verifique se o nome est� correto no LevelData e se o prefab foi arrastado para a 'Enemy Prefabs Library' do WaveSpawner.", this.gameObject);
        }
    }
}