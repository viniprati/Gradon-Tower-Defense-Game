// RankingUIController.cs (Versão Final, Segura e Corrigida)

using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RankingUIController : MonoBehaviour
{
    [Header("Referências Top 3")]
    [SerializeField] private TextMeshProUGUI top1_Text;
    [SerializeField] private TextMeshProUGUI top2_Text;
    [SerializeField] private TextMeshProUGUI top3_Text;

    [Header("Referências Outros (4º ao 10º)")]
    [SerializeField] private TextMeshProUGUI otherRanks_TemplateText;
    [SerializeField] private Transform otherRanks_ContainerParent;

    void Start()
    {
        PopulateRanking();
    }

    public void PopulateRanking()
    {
        if (RankingManager.instance == null)
        {
            Debug.LogError("RankingManager não encontrado! A UI de Ranking será desativada.", this.gameObject);
            gameObject.SetActive(false);
            return;
        }

        List<ScoreEntry> ranking = RankingManager.instance.GetRanking();

        if (top1_Text != null)
            top1_Text.text = ranking.Count > 0 ? $"1. {ranking[0].playerName} - {ranking[0].score}" : "1. ...";
        if (top2_Text != null)
            top2_Text.text = ranking.Count > 1 ? $"2. {ranking[1].playerName} - {ranking[1].score}" : "2. ...";
        if (top3_Text != null)
            top3_Text.text = ranking.Count > 2 ? $"3. {ranking[2].playerName} - {ranking[2].score}" : "3. ...";

        if (otherRanks_ContainerParent == null)
        {
            Debug.LogError("'otherRanks_ContainerParent' não está atribuído no Inspector! Verifique o objeto " + this.gameObject.name, this.gameObject);
            return;
        }
        if (otherRanks_TemplateText == null)
        {
            Debug.LogError("'otherRanks_TemplateText' não está atribuído no Inspector!", this.gameObject);
            return;
        }

        foreach (Transform child in otherRanks_ContainerParent)
        {
            Destroy(child.gameObject);
        }

        otherRanks_TemplateText.gameObject.SetActive(true);

        if (ranking.Count > 3)
        {
            for (int i = 3; i < ranking.Count; i++)
            {
                GameObject rankEntryGO = Instantiate(otherRanks_TemplateText.gameObject, otherRanks_ContainerParent);
                TextMeshProUGUI rankEntryText = rankEntryGO.GetComponent<TextMeshProUGUI>();

                int rankNumber = i + 1;
                rankEntryText.text = $"{rankNumber}. {ranking[i].playerName} - {ranking[i].score}";
            }
        }

        otherRanks_TemplateText.gameObject.SetActive(false);
    }
}