// Uma classe simples para guardar os dados de cada entrada do ranking.
// [System.Serializable] permite que o Unity a converta para JSON.
[System.Serializable]
public class ScoreData
{
    public string playerName;
    public int score;
}