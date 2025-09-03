// WaveSpawner.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Necessário para usar Listas, se preferir

public class WaveSpawner : MonoBehaviour
{
    [Header("Configuração das Ondas")]
    [Tooltip("Crie e configure todas as ondas para esta fase aqui no Inspector.")]
    public Wave[] waves; // Um array com todas as ondas da fase

    [Header("Referências")]
    [Tooltip("Arraste todos os objetos que servirão como pontos de spawn para esta lista.")]
    public Transform[] spawnPoints;

    // Você pode adicionar referências para a UI aqui, para mostrar "Onda 1/5", etc.
    // public Text waveCounterText;

    /// <summary>
    /// Chamado pela Unity quando o jogo começa.
    /// </summary>
    void Start()
    {
        // Checagem de segurança para garantir que os pontos de spawn foram configurados.
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
    /// A rotina principal (Coroutine) que gerencia o fluxo de todas as ondas, uma após a outra.
    /// </summary>
    private IEnumerator SpawnAllWaves()
    {
        // Loop que passa por cada onda que você criou no Inspector.
        for (int i = 0; i < waves.Length; i++)
        {
            Wave currentWave = waves[i];

            // Espera o tempo de preparação definido ANTES da onda começar.
            yield return new WaitForSeconds(currentWave.delayBeforeWave);

            // Mensagem no console para sabermos qual onda está começando.
            Debug.Log($"<color=orange>Iniciando Onda {i + 1}: {currentWave.waveName}</color>");

            // Chama a rotina que vai spawnar os inimigos desta onda específica e espera ela terminar.
            yield return StartCoroutine(SpawnCurrentWave(currentWave));
        }

        // Esta parte do código só é alcançada depois que TODAS as ondas terminarem.
        Debug.Log("<color=green>FASE CONCLUÍDA! Todas as ondas foram derrotadas!</color>");
        // Adicione aqui a lógica de vitória da fase (ex: mostrar uma tela de "Você Venceu!").
    }

    /// <summary>
    /// Rotina que gerencia o spawn de todos os grupos de inimigos de uma única onda.
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

                // Espera o intervalo de tempo definido antes de spawnar o próximo inimigo do MESMO grupo.
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
    }

    /// <summary>
    /// Spawna um único inimigo em um dos pontos de spawn aleatórios.
    /// </summary>
    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Tentativa de spawnar um inimigo, mas o prefab é nulo. Verifique a configuração das ondas no Inspector.");
            return;
        }

        // Escolhe um ponto de spawn aleatório da lista que você configurou.
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Cria (instancia) o inimigo na posição e rotação do ponto de spawn escolhido.
        Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
    }
}