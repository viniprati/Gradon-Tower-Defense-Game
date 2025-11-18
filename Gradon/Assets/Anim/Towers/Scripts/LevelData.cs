using UnityEngine;
using System.Collections.Generic;

// A classe EnemyGroup define as propriedades de um subgrupo de inimigos dentro de uma onda.
// [System.Serializable] permite que vejamos e editemos suas variáveis no Inspector da Unity.
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

// A classe Wave define uma única onda de ataque, que pode conter vários grupos de inimigos.
[System.Serializable]
public class Wave
{
    [Tooltip("Apenas um nome para você se organizar no Inspector.")]
    public string waveName;

    [Tooltip("A lista de grupos de inimigos que compõem esta onda.")]
    public List<EnemyGroup> enemyGroups;

    [Tooltip("O tempo de espera em segundos ANTES desta onda começar.")]
    public float delayBeforeWave;
}

// O ScriptableObject principal que armazena todos os dados de uma fase específica.
// [CreateAssetMenu] permite que você crie novos arquivos de fase diretamente no menu do Unity
// (clique com o botão direito na pasta Project > Create > Tower Defense > Level Data).
[CreateAssetMenu(fileName = "New LevelData", menuName = "Tower Defense/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Informações da Fase")]
    [Tooltip("O nome que aparecerá para o jogador no menu de seleção.")]
    public string levelName = "Nova Fase";

    [Tooltip("O número de identificação da fase (1, 2, 3...). Usado para ordenar a lista de fases.")]
    public int levelIndex;

    [Tooltip("O nome exato do arquivo de cena a ser carregado para esta fase.")]
    public string sceneToLoad;

    [Header("Configuração das Ondas")]
    [Tooltip("Configure aqui todas as ondas de inimigos para esta fase.")]
    public List<Wave> waves;

    [Header("Recursos Iniciais")]
    [Tooltip("A quantidade de mana com que o jogador começa a fase.")]
    // Esta variável corresponde diretamente à lógica de inicialização de mana no GameManager.
    public float startingMana = 150f;
}