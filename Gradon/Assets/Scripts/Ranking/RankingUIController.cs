// RankingUIController.cs (Vers�o Corrigida e Segura)

using UnityEngine;
using TMPro;
using System.Collections.Generic;
// Removido 'using UnityEngine.SceneManagement;' pois este script n�o deve mais controlar cenas.

public class RankingUIController : MonoBehaviour
{
    [Header("Refer�ncias Top 3")]
    [Tooltip("Texto para o 1� lugar. Ex: '1. NOME - SCORE'.")]
    [SerializeField] private TextMeshProUGUI top1_Text;
    [SerializeField] private TextMeshProUGUI top2_Text;
    [SerializeField] private TextMeshProUGUI top3_Text;

    [Header("Refer�ncias Outros (4� ao 10�)")]
    [Tooltip("Um �nico objeto TextMeshProUGUI que ser� usado como template.")]
    [SerializeField] private TextMeshProUGUI otherRanks_TemplateText;
    [Tooltip("O 'pai' onde os textos do 4� ao 10� lugar ser�o clonados.")]
    [SerializeField] private Transform otherRanks_ContainerParent;

    void Start()
    {
        // A l�gica de popular o ranking s� deve acontecer se o painel estiver vis�vel.
        // Se voc� tiver um bot�o para mostrar o ranking, mova a chamada 'PopulateRanking()'
        // para a fun��o daquele bot�o. Por enquanto, vamos deixar no Start.
        PopulateRanking();
    }

    // Tornamos o m�todo p�blico para que possa ser chamado por um bot�o "Atualizar" se necess�rio.
    public void PopulateRanking()
    {
        // --- VERIFICA��O DE SEGURAN�A #1: RankingManager ---
        if (RankingManager.instance == null)
        {
            Debug.LogError("RankingManager n�o encontrado na cena! A UI de Ranking ser� desativada.", this.gameObject);
            gameObject.SetActive(false); // Desativa o objeto para evitar mais erros.
            return;
        }

        // Pega a lista de scores ordenada.
        List<ScoreEntry> ranking = RankingManager.instance.GetRanking();

        // --- VERIFICA��O DE SEGURAN�A #2: Refer�ncias do Top 3 ---
        // Se as refer�ncias do Top 3 n�o estiverem atribu�das, apenas mostra um aviso e continua.
        if (top1_Text == null || top2_Text == null || top3_Text == null)
        {
            Debug.LogWarning("Uma ou mais refer�ncias de texto do Top 3 n�o foram atribu�das no Inspector.", this.gameObject);
        }
        else
        {
            top1_Text.text = ranking.Count > 0 ? $"1. {ranking[0].playerName} - {ranking[0].score}" : "1. ...";
            top2_Text.text = ranking.Count > 1 ? $"2. {ranking[1].playerName} - {ranking[1].score}" : "2. ...";
            top3_Text.text = ranking.Count > 2 ? $"3. {ranking[2].playerName} - {ranking[2].score}" : "3. ...";
        }

        // --- VERIFICA��O DE SEGURAN�A #3: Refer�ncias dos Outros Ranks ---
        // Se o container ou o template n�o estiverem atribu�dos, o c�digo para aqui,
        // mas sem recarregar a cena.
        if (otherRanks_ContainerParent == null)
        {
            Debug.LogError("'otherRanks_ContainerParent' n�o est� atribu�do no Inspector! N�o � poss�vel popular o resto do ranking.", this.gameObject);
            return; // PARA A EXECU��O DO M�TODO AQUI.
        }
        if (otherRanks_TemplateText == null)
        {
            Debug.LogError("'otherRanks_TemplateText' n�o est� atribu�do no Inspector! N�o � poss�vel popular o resto do ranking.", this.gameObject);
            return; // PARA A EXECU��O DO M�TODO AQUI.
        }

        // Limpa o conte�do antigo antes de adicionar o novo.
        foreach (Transform child in otherRanks_ContainerParent)
        {
            Destroy(child.gameObject);
        }

        // Ativa o template para o caso de ele estar desativado no editor.
        otherRanks_TemplateText.gameObject.SetActive(true);

        // Preenche os outros (do 4� ao 10� lugar).
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

        // Desativa o template original para que ele n�o apare�a na lista final.
        otherRanks_TemplateText.gameObject.SetActive(false);
    }

    // A fun��o GoToGame() foi REMOVIDA para centralizar o controle de cenas
    // no MenuManager e LevelButton, evitando o recarregamento indesejado da cena.
}