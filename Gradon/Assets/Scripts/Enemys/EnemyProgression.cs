// Esta pequena classe/struct nos ajuda a organizar a progressão dos inimigos no Inspector.
// [System.Serializable] faz com que ela apareça no Inspector da Unity.
using UnityEngine;

[System.Serializable]
public class EnemyProgression
{
    public string description; // Apenas para organização no Inspector
    public GameObject enemyPrefab;
    [Tooltip("Em quantos segundos de jogo este inimigo começa a aparecer.")]
    public float timeToStartSpawning;
}