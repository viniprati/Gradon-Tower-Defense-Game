// WaveSpawner.cs (Adaptado para a sua estrutura de LevelData)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner instance;

    [Header("Configura��o de Spawn")]
    [SerializeField] private Transform[] spawnPoints;

    // Removi o timeBetweenWaves daqui, pois a sua estrutura j� tem 'delayBeforeWave'

    private List<Wave> _wavesToSpawn;
    private int _currentWaveIndex = -1;
    private int _enemiesAlive = 0;
    private bool _levelIsActive = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        LoadLevelData();
    }

    private void LoadLevelData()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentLevelData != null)
        {
            LevelData currentLevel = GameManager.Instance.currentLevelData;
            _wavesToSpawn = currentLevel.waves;
            Debug.Log($"Fase '{currentLevel.name}' carregada com {_wavesToSpawn.Count} ondas.");

            // Inicia a primeira onda
            _levelIsActive = true;
            StartNextWave();
        }
        else
        {
            Debug.LogError("WaveSpawner n�o conseguiu carregar LevelData do GameManager!");
        }
    }

    private void StartNextWave()
    {
        if (!_levelIsActive) return;

        _currentWaveIndex++;

        if (_currentWaveIndex < _wavesToSpawn.Count)
        {
            StartCoroutine(SpawnWaveCoroutine(_wavesToSpawn[_currentWaveIndex]));
        }
        else
        {
            _levelIsActive = false;
            Debug.Log("Todas as ondas foram geradas! Aguardando inimigos restantes.");
        }
    }

    private IEnumerator SpawnWaveCoroutine(Wave wave)
    {
        Debug.Log($"Aguardando {wave.delayBeforeWave}s antes da onda '{wave.waveName}'.");
        yield return new WaitForSeconds(wave.delayBeforeWave);

        Debug.Log($"Iniciando Onda {_currentWaveIndex + 1}: '{wave.waveName}'.");

        // Loop atrav�s de cada GRUPO de inimigos dentro da onda
        foreach (EnemyGroup group in wave.enemyGroups)
        {
            // Inicia uma corrotina separada para cada grupo, para que eles possam spawnar em paralelo
            StartCoroutine(SpawnEnemyGroupCoroutine(group));
        }
    }

    private IEnumerator SpawnEnemyGroupCoroutine(EnemyGroup group)
    {
        for (int i = 0; i < group.count; i++)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Instantiate(group.enemyPrefab, randomSpawnPoint.position, Quaternion.identity);
            _enemiesAlive++;

            yield return new WaitForSeconds(group.spawnInterval);
        }
    }

    public void OnEnemyDied()
    {
        _enemiesAlive--;

        // Se todas as ondas j� foram iniciadas e n�o h� mais inimigos... vit�ria!
        if (!_levelIsActive && _enemiesAlive <= 0)
        {
            GameManager.Instance.HandleGameOver(true);
        }
        // Se a onda atual acabou (n�o h� mais inimigos vivos) E ainda h� mais ondas para vir...
        else if (_levelIsActive && _enemiesAlive <= 0 && _currentWaveIndex >= _wavesToSpawn.Count - 1)
        {
            // Este caso � para a �ltima onda. A vit�ria ser� tratada pela condi��o acima.
            // Mas se precisar de uma l�gica especial quando a �ltima onda � limpa, coloque aqui.
        }
    }
}