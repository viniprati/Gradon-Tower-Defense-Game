// RankingUIController.cs (Vers�o Simplificada - Apenas Top 3)

using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RankingUIController : MonoBehaviour
{
    [Header("Refer�ncias Top 3")]
    [Tooltip("Texto para o 1� lugar. Ex: '1. NOME - SCORE'.")]
    [SerializeField] private TextMeshProUGUI top1_Text;

    [Tooltip("Texto para o 2� lugar.")]
    [SerializeField] private TextMeshProUGUI top2_Text;

    [Tooltip("Texto para o 3� lugar.")]
    [SerializeField] private TextMeshProUGUI top3_Text;

    void Start()
    {
        PopulateRanking();
    }

    public void PopulateRanking()
    {
        // --- Verifica��o de Seguran�a #1: RankingManager ---
        // Garante que o RankingManager existe na cena.
        if (RankingManager.instance == null)
        {
            Debug.LogError("RankingManager n�o encontrado! A UI de Ranking ser� desativada.", this.gameObject);
            gameObject.SetActive(false); // Desativa este objeto para evitar mais erros.
            return;
        }

        // Pega a lista de scores do RankingManager.
        List<ScoreEntry> ranking = RankingManager.instance.GetRanking();

        // --- Verifica��o de Seguran�a #2: Refer�ncias de Texto ---
        // Checa se os campos de texto foram arrastados no Inspector.
        if (top1_Text == null || top2_Text == null || top3_Text == null)
        {
            Debug.LogError("Uma ou mais refer�ncias de texto do Top 3 n�o foram atribu�das no Inspector!", this.gameObject);
            return; // Para a execu��o para evitar erros.
        }

        // --- Preenche o Ranking do Top 3 ---
        // Usa um operador tern�rio para preencher o texto ou mostrar "..." se n�o houver score.
        top1_Text.text = ranking.Count > 0 ? $"1. {ranking[0].playerName} - {ranking[0].score}" : "1. ...";
        top2_Text.text = ranking.Count > 1 ? $"2. {ranking[1].playerName} - {ranking[1].score}" : "2. ...";
        top3_Text.text = ranking.Count > 2 ? $"3. {ranking[2].playerName} - {ranking[2].score}" : "3. ...";
    }
}