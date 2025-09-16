// RankingData.cs

// [System.Serializable] permite que estas classes sejam convertidas para JSON,
// o que é essencial para salvar e carregar os dados.

[System.Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;
}

[System.Serializable]
public class ScoreList
{
    // C# precisa de uma classe "wrapper" para serializar uma lista.
    public System.Collections.Generic.List<ScoreEntry> scores = new System.Collections.Generic.List<ScoreEntry>();
}