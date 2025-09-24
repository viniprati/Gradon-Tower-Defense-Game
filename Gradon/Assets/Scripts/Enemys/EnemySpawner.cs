// EnemySpawner.cs (Vers�o Final e Corrigida)

using UnityEngine;
using System.Collections;
using System.Collections.Generic; // <-- CORRE��O: Adicionado para usar List<>
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public static event System.Action OnAllWavesCompleted;

    [Header("Refer�ncias da Cena")]
    public Transform[] spawnPoints;

    [Header("Refer�ncias da UI")]
    public TextMeshProUGUI waveInfoText;

    [Header("Debug / Teste R�pido")]
    [Tooltip("Arraste um arquivo de fase aqui para testar esta cena diretamente.")]
    public LevelData debugLevel;

    private int enemiesAlive = 0;

    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Nenhum ponto de spawn configurado!", this.gameObject);
            this.enabled = false;
            return;
        }

        // <-- CORRE��O: A vari�vel wavesToSpawn agora � do tipo List<Wave>
        List<Wave> wavesToSpawn = null;

        if (GameManager.instance != null && GameManager.instance.currentLevelData != null)
        {
            // Nenhuma mudan�a aqui, pois a atribui��o de List para List funciona
            wavesToSpawn = GameManager.instance.currentLevelData.waves;
        }
        else if (debugLevel != null)
        {
            Debug.LogWarning("GameManager n�o encontrado. Carregando fase de DEBUG: " + debugLevel.name, this.gameObject);
            // Nenhuma mudan�a aqui tamb�m, a l�gica continua a mesma
            wavesToSpawn = debugLevel.waves;
        }

        // <-- CORRE��O: A verifica��o agora usa .Count em vez de .Length
        if (wavesToSpawn != null && wavesToSpawn.Count > 0)
        {
            StartCoroutine(SpawnAllWaves(wavesToSpawn));
        }
        else
        {
            Debug.LogError("Nenhuma onda para spawnar! Verifique se a lista 'Waves' est� preenchida no seu arquivo LevelData.", this.gameObject);
            this.enabled = false;
        }
    }

    // <-- CORRE��O: O par�metro do m�todo agora aceita List<Wave>
    private IEnumerator SpawnAllWaves(List<Wave> waves)
    {
        // <-- CORRE��O: Usamos waves.Count para o loop
        for (int i = 0; i < waves.Count; i++)
        {
            Wave currentWave = waves[i];

            if (waveInfoText != null)
                waveInfoText.text = $"Pr�xima Onda em {currentWave.delayBeforeWave:F1}s...";

            yield return new WaitForSeconds(currentWave.delayBeforeWave);

            if (waveInfoText != null)
                // <-- CORRE��O: Usamos waves.Count para a UI
                waveInfoText.text = $"Onda {i + 1} / {waves.Count}";

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
        // Nenhuma mudan�a necess�ria aqui, o foreach funciona com List e Array
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
            Debug.LogWarning("Tentando spawnar um inimigo, mas o prefab est� nulo. Verifique a configura��o da onda no seu arquivo de fase.", this.gameObject);
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