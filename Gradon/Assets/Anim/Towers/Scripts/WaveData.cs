// WaveData.cs

using UnityEngine;

// [System.Serializable] é o que permite que estas classes apareçam no Inspector da Unity.

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab; // Qual inimigo spawnar
    public int count;              // Quantos inimigos deste tipo
    public float spawnInterval;    // Intervalo de tempo entre cada inimigo deste grupo
}

[System.Serializable]
public class Wave
{
    public string waveName;           // Nome da onda (para sua organização)
    public EnemyGroup[] enemyGroups;  // Uma lista dos grupos de inimigos que formam esta onda
    public float delayBeforeWave;     // Tempo de espera em segundos ANTES desta onda começar
}