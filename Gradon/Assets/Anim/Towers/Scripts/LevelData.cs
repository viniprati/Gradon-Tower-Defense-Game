// LevelData.cs (Vers�o Corrigida para usar List<>)

using UnityEngine;
// Adicionamos esta linha para poder usar List<>
using System.Collections.Generic;

[System.Serializable]
public class EnemyGroup
{
    [Tooltip("O prefab do inimigo a ser criado.")]
    public GameObject enemyPrefab;

    [Tooltip("Quantos inimigos deste tipo ser�o criados neste grupo.")]
    public int count;

    [Tooltip("O intervalo em segundos entre a cria��o de cada inimigo deste grupo.")]
    public float spawnInterval;
}

[System.Serializable]
public class Wave
{
    [Tooltip("Apenas um nome para voc� se organizar no Inspector.")]
    public string waveName;

    [Tooltip("A lista de grupos de inimigos que comp�em esta onda.")]
    // CORRE��O AQUI: Mudamos de Array (EnemyGroup[]) para Lista (List<EnemyGroup>)
    public List<EnemyGroup> enemyGroups;

    [Tooltip("O tempo de espera em segundos ANTES desta onda come�ar.")]
    public float delayBeforeWave;
}

[CreateAssetMenu(fileName = "New Level", menuName = "Tower Defense/Level")]
public class LevelData : ScriptableObject
{
    [Header("Informa��es da Fase")]
    [Tooltip("O nome que aparecer� para o jogador.")]
    public string levelName = "Nova Fase";

    [Tooltip("O n�mero de identifica��o da fase (1, 2, 3...).")]
    public int levelIndex;

    [Tooltip("O nome exato do arquivo de cena a ser carregado para esta fase.")]
    public string sceneToLoad;

    [Header("Configura��o das Ondas")]
    [Tooltip("Configure aqui todas as ondas de inimigos para esta fase.")]
    // CORRE��O AQUI: Mudamos de Array (Wave[]) para Lista (List<Wave>)
    public List<Wave> waves;

    [Header("Recursos Iniciais")]
    [Tooltip("A quantidade de mana com que o jogador come�a a fase.")]
    public int initialMana = 150;
}