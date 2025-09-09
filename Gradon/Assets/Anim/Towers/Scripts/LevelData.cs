// LevelData.cs
using UnityEngine;

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnInterval;
}

[System.Serializable]
public class Wave
{
    public string waveName;
    public EnemyGroup[] enemyGroups;
    public float delayBeforeWave;
}

[CreateAssetMenu(fileName = "New Level", menuName = "Tower Defense/Level")]
public class LevelData : ScriptableObject
{
    [Header("Informa��es da Fase")]
    public string levelName = "Nova Fase";
    public int levelIndex;
    public string sceneToLoad;

    [Header("Configura��o das Ondas")]
    public Wave[] waves;

    [Header("Recursos Iniciais")]
    public int initialMana = 150;
}