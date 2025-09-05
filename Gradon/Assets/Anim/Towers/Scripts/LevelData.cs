// LevelData.cs

using UnityEngine;

// O atributo CreateAssetMenu nos permite criar "arquivos de fase"
// diretamente no menu da Unity (Assets > Create > Level).
[CreateAssetMenu(fileName = "New Level", menuName = "Tower Defense/Level")]
public class LevelData : ScriptableObject
{
    [Header("Informa��es da Fase")]
    public string levelName = "Nova Fase";
    public int levelIndex; // Ex: 1, 2, 3...
    public string sceneToLoad; // O nome da cena/mapa a ser carregado

    [Header("Configura��o das Ondas")]
    [Tooltip("Todas as ondas de inimigos para esta fase.")]
    public Wave[] waves; // Reutilizamos a classe 'Wave' que j� criamos!

    [Header("Recompensas")]
    public int manaInicial = 150;
    // Voc� pode adicionar mais recompensas aqui, como "torres desbloqueadas", etc.
}