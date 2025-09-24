// WaveData.cs

// Este 'using' é necessário para usar tipos da Unity como GameObject.
using UnityEngine;

// A diretiva [System.Serializable] é a parte mais importante deste script.
// Ela diz à Unity que objetos desta classe podem ser "serializados",
// o que significa que eles podem ser salvos e exibidos no Inspector.
// Sem isso, a lista de ondas no seu LevelData ficaria invisível.
[System.Serializable]
public class WaveData
{
    // --- Variáveis Públicas (Aparecerão no Inspector) ---

    [Tooltip("Nome opcional para identificar esta onda no editor.")]
    public string waveName;

    [Tooltip("Arraste o Prefab do inimigo que será gerado nesta onda.")]
    public GameObject enemyPrefab;

    [Tooltip("Quantos inimigos deste tipo serão gerados nesta onda.")]
    [Min(1)] // Garante que o valor no Inspector não pode ser menor que 1.
    public int count = 10;

    [Tooltip("O intervalo de tempo (em segundos) entre cada inimigo gerado.")]
    [Range(0.1f, 5f)] // Cria um slider no Inspector para facilitar o ajuste.
    public float spawnInterval = 1f;
}