// WaveData.cs

using UnityEngine;

/// <summary>
/// Define a estrutura de um grupo de inimigos dentro de uma onda.
/// </summary>
[System.Serializable]
public class EnemyGroup
{
    [Tooltip("Digite o NOME EXATO do prefab do inimigo (ex: 'NormalEnemy', 'RangedEnemy'). O nome deve corresponder ao que est� na 'Enemy Prefabs Library' do WaveSpawner.")]
    public string enemyType; // O nome do prefab a ser spawnado

    [Tooltip("Quantos inimigos deste tipo ser�o criados neste grupo.")]
    public int count;

    [Tooltip("O intervalo em segundos entre a cria��o de cada inimigo deste grupo.")]
    public float spawnInterval;
}

/// <summary>
/// Define a estrutura de uma �nica onda de inimigos.
/// </summary>
[System.Serializable]
public class Wave
{
    [Tooltip("Apenas um nome para voc� se organizar no Inspector.")]
    public string waveName;

    [Tooltip("A lista de grupos de inimigos que comp�em esta onda.")]
    public EnemyGroup[] enemyGroups;

    [Tooltip("O tempo de espera em segundos ANTES desta onda come�ar.")]
    public float delayBeforeWave;
}