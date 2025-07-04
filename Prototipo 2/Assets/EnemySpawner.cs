// EnemySpawner.cs
using UnityEngine;
using System.Collections; // Essencial para usar Coroutines

public class EnemySpawner : MonoBehaviour
{
    [Header("Configurações do Spawner")]
    [Tooltip("O prefab do Ghoul que será gerado.")]
    [SerializeField] private GameObject ghoulPrefab;

    [Tooltip("O tempo, em segundos, entre cada Ghoul gerado.")]
    [SerializeField] private float spawnInterval = 3f;

    [Tooltip("Um atraso inicial antes do primeiro Ghoul aparecer.")]
    [SerializeField] private float initialDelay = 2f;

    void Start()
    {
        // Inicia o processo de geração de inimigos.
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // Verifica se o prefab foi atribuído para evitar erros.
        if (ghoulPrefab == null)
        {
            Debug.LogError("O prefab do Ghoul não foi atribuído no Spawner! Arraste-o no Inspector.");
            yield break; // Para a coroutine se não houver prefab.
        }

        // 1. Espera o tempo de atraso inicial.
        yield return new WaitForSeconds(initialDelay);

        // 2. Loop infinito para gerar inimigos continuamente.
        while (true)
        {
            // 3. Cria uma instância do prefab do Ghoul na posição do Spawner.
            Instantiate(ghoulPrefab, transform.position, Quaternion.identity);

            // 4. Espera o intervalo de tempo definido antes de continuar o loop.
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}