// WaveData.cs

// Este 'using' � necess�rio para usar tipos da Unity como GameObject.
using UnityEngine;

// A diretiva [System.Serializable] � a parte mais importante deste script.
// Ela diz � Unity que objetos desta classe podem ser "serializados",
// o que significa que eles podem ser salvos e exibidos no Inspector.
// Sem isso, a lista de ondas no seu LevelData ficaria invis�vel.
[System.Serializable]
public class WaveData
{
    // --- Vari�veis P�blicas (Aparecer�o no Inspector) ---

    [Tooltip("Nome opcional para identificar esta onda no editor.")]
    public string waveName;

    [Tooltip("Arraste o Prefab do inimigo que ser� gerado nesta onda.")]
    public GameObject enemyPrefab;

    [Tooltip("Quantos inimigos deste tipo ser�o gerados nesta onda.")]
    [Min(1)] // Garante que o valor no Inspector n�o pode ser menor que 1.
    public int count = 10;

    [Tooltip("O intervalo de tempo (em segundos) entre cada inimigo gerado.")]
    [Range(0.1f, 5f)] // Cria um slider no Inspector para facilitar o ajuste.
    public float spawnInterval = 1f;
}