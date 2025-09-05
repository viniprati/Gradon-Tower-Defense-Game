// LevelData.cs

using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Tower Defense/Level")]
public class LevelData : ScriptableObject
{
    [Header("Informações da Fase")]
    public string levelName = "Nova Fase";
    public int levelIndex; 
    public string sceneToLoad; 

    [Header("Configuração das Ondas")]
    [Tooltip("Todas as ondas de inimigos para esta fase.")]
    public Wave[] waves; 

    [Header("Recompensas")]
    public int manaInicial = 150;
}