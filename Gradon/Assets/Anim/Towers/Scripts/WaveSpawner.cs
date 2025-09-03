// WaveSpawner.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Necess�rio para usar Listas, se preferir

public class WaveSpawner : MonoBehaviour
{
    [Header("Configura��o das Ondas")]
    [Tooltip("Crie e configure todas as ondas para esta fase aqui no Inspector.")]
    public Wave[] waves; // Um array com todas as ondas da fase

    [Header("Refer�ncias")]
    [Tooltip("Arraste todos os objetos que servir�o como pontos de spawn para esta lista.")]
    public Transform[] spawnPoints;

    // Voc� pode adicionar refer�ncias para a UI aqui, para mostrar "Onda 1/5", etc.
    // public Text waveCounterText;

    /// <summary>
    /// Chamado pela Unity quando o jogo come�a.
    /// </summary>
    void Start()
    {
        // Checagem de seguran�a para garantir que os pontos de spawn foram configurados.
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("ERRO: Nenhum ponto de spawn foi configurado no WaveSpawner! Arraste os objetos para a lista no Inspector.", this.gameObject);
            this.enabled = false; // Desativa o script para evitar mais erros.
            return;
        }

        // Inicia a rotina principal que vai controlar o fluxo de todas as ondas.
        StartCoroutine(SpawnAllWaves());
    }

    /// <summary>
    /// A rotina principal (Coroutine) que gerencia o fluxo de todas as ondas, uma ap�s a outra.
    /// </summary>
    private IEnumerator SpawnAllWaves()
    {
        // Loop que passa por cada onda que voc� criou no Inspector.
        for (int i = 0; i < waves.Length; i++)
        {
            Wave currentWave = waves[i];

            // Espera o tempo de prepara��o definido ANTES da onda come�ar.
            yield return new WaitForSeconds(currentWave.delayBeforeWave);

            // Mensagem no console para sabermos qual onda est� come�ando.
            Debug.Log($"<color=orange>Iniciando Onda {i + 1}: {currentWave.waveName}</color>");

            // Chama a rotina que vai spawnar os inimigos desta onda espec�fica e espera ela terminar.
            yield return StartCoroutine(SpawnCurrentWave(currentWave));
        }

        // Esta parte do c�digo s� � alcan�ada depois que TODAS as ondas terminarem.
        Debug.Log("<color=green>FASE CONCLU�DA! Todas as ondas foram derrotadas!</color>");
        // Adicione aqui a l�gica de vit�ria da fase (ex: mostrar uma tela de "Voc� Venceu!").
    }

    /// <summary>
    /// Rotina que gerencia o spawn de todos os grupos de inimigos de uma �nica onda.
    /// </summary>
    private IEnumerator SpawnCurrentWave(Wave wave)
    {
        // Loop que passa por cada grupo de inimigos (ex: "5 zumbis", "2 ogros") dentro da onda.
        foreach (EnemyGroup group in wave.enemyGroups)
        {
            // Spawna todos os inimigos do grupo atual, um por um.
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(group.enemyPrefab);

                // Espera o intervalo de tempo definido antes de spawnar o pr�ximo inimigo do MESMO grupo.
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }

    /// <summary>
    /// Spawna um �nico inimigo em um dos pontos de spawn aleat�rios.
    /// </summary>
    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Tentativa de spawnar um inimigo, mas o prefab � nulo. Verifique a configura��o das ondas no Inspector.");
            return;
        }

        // Escolhe um ponto de spawn aleat�rio da lista que voc� configurou.
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Cria (instancia) o inimigo na posi��o e rota��o do ponto de spawn escolhido.
        Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
    }
}