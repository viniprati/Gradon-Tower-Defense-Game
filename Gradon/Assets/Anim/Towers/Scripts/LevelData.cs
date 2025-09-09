// LevelData.cs

using UnityEngine;

// [System.Serializable] é o que permite que estas classes customizadas (EnemyGroup e Wave)
// apareçam de forma organizada dentro do Inspector da Unity.

[System.Serializable]
public class EnemyGroup
{
    [Tooltip("O prefab do inimigo a ser criado.")]
    public GameObject enemyPrefab;

    [Tooltip("Quantos inimigos deste tipo serão criados neste grupo.")]
    public int count;

    [Tooltip("O intervalo em segundos entre a criação de cada inimigo deste grupo.")]
    public float spawnInterval;
}

[System.Serializable]
public class Wave
{
    [Tooltip("Apenas um nome para você se organizar no Inspector.")]
    public string waveName;

    [Tooltip("A lista de grupos de inimigos que compõem esta onda.")]
    public EnemyGroup[] enemyGroups;

    [Tooltip("O tempo de espera em segundos ANTES desta onda começar.")]
    public float delayBeforeWave;
}


// [CreateAssetMenu] é a instrução que adiciona a opção para criar este tipo de asset
// no menu da Unity (Assets > Create > Tower Defense > Level).
// A classe herda de 'ScriptableObject', transformando este script em um "molde" para
// criar arquivos de dados, em vez de um componente de cena (MonoBehaviour).
[CreateAssetMenu(fileName = "New Level", menuName = "Tower Defense/Level")]
public class LevelData : ScriptableObject
{
    [Header("Informações da Fase")]
    [Tooltip("O nome que aparecerá para o jogador.")]
    public string levelName = "Nova Fase";

    [Tooltip("O número de identificação da fase (1, 2, 3...).")]
    public int levelIndex;

    [Tooltip("O nome exato do arquivo de cena a ser carregado para esta fase.")]
    public string sceneToLoad;

    [Header("Configuração das Ondas")]
    [Tooltip("Configure aqui todas as ondas de inimigos para esta fase.")]
    public Wave[] waves;

    [Header("Recursos Iniciais")]
    [Tooltip("A quantidade de mana com que o jogador começa a fase.")]
    public int initialMana = 150;
}