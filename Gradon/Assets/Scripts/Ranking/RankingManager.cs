// RankingManager.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Necessário para usar OrderByDescending

// As definições de 'ScoreEntry' e 'ScoreList' foram removidas deste arquivo
// para evitar erros de ambiguidade, assumindo que elas já existem em outro script no seu projeto.

public class RankingManager : MonoBehaviour
{
    // A instância ainda é útil para ser acessada por outros scripts DENTRO DA MESMA CENA.
    public static RankingManager instance;

    private ScoreList rankingData;
    private const string RankingKey = "GameRanking"; // A chave para salvar nos PlayerPrefs

    void Awake()
    {
        // Padrão Singleton que NÃO persiste entre as cenas,
        // evitando conflitos com o GameManager.
        if (instance == null)
        {
            instance = this;
            LoadRanking(); // Carrega os dados salvos ao iniciar a cena de menu
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadRanking()
    {
        if (PlayerPrefs.HasKey(RankingKey))
        {
            string json = PlayerPrefs.GetString(RankingKey);
            rankingData = JsonUtility.FromJson<ScoreList>(json);
        }
        else
        {
            rankingData = new ScoreList();
        }
    }

    private void SaveRanking()
    {
        rankingData.scores = rankingData.scores.OrderByDescending(entry => entry.score).ToList();

        if (rankingData.scores.Count > 10)
        {
            rankingData.scores = rankingData.scores.GetRange(0, 10);
        }

        string json = JsonUtility.ToJson(rankingData);
        PlayerPrefs.SetString(RankingKey, json);
        PlayerPrefs.Save();
    }

    public void AddScore(string playerName, int score)
    {
        // Se rankingData for nulo, inicialize-o para evitar erros.
        if (rankingData == null) { rankingData = new ScoreList(); }
        if (rankingData.scores == null) { rankingData.scores = new List<ScoreEntry>(); }

        ScoreEntry newEntry = new ScoreEntry { playerName = playerName, score = score };
        rankingData.scores.Add(newEntry);
        SaveRanking();
    }

    public List<ScoreEntry> GetRanking()
    {
        // Garante que não retornará nulo se o ranking ainda não foi carregado.
        if (rankingData == null || rankingData.scores == null)
        {
            return new List<ScoreEntry>();
        }
        return rankingData.scores;
    }

    public void ClearRanking()
    {
        if (rankingData != null && rankingData.scores != null)
        {
            rankingData.scores.Clear();
        }
        PlayerPrefs.DeleteKey(RankingKey);
        Debug.Log("Ranking foi limpo!");
    }
}