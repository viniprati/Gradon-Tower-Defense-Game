// RankingUIController.cs (Versão Corrigida e Segura)

using UnityEngine;
using TMPro;
using System.Collections.Generic;
// Removido 'using UnityEngine.SceneManagement;' pois este script não deve mais controlar cenas.

public class RankingUIController : MonoBehaviour
{
    [Header("Referências Top 3")]
    [Tooltip("Texto para o 1º lugar. Ex: '1. NOME - SCORE'.")]
    [SerializeField] private TextMeshProUGUI top1_Text;
    [SerializeField] private TextMeshProUGUI top2_Text;
    [SerializeField] private TextMeshProUGUI top3_Text;

    [Header("Referências Outros (4º ao 10º)")]
    [Tooltip("Um único objeto TextMeshProUGUI que será usado como template.")]
    [SerializeField] private TextMeshProUGUI otherRanks_TemplateText;
    [Tooltip("O 'pai' onde os textos do 4º ao 10º lugar serão clonados.")]
    [SerializeField] private Transform otherRanks_ContainerParent;

    void Start()
    {
        // A lógica de popular o ranking só deve acontecer se o painel estiver visível.
        // Se você tiver um botão para mostrar o ranking, mova a chamada 'PopulateRanking()'
        // para a função daquele botão. Por enquanto, vamos deixar no Start.
        PopulateRanking();
    }

    // Tornamos o método público para que possa ser chamado por um botão "Atualizar" se necessário.
    public void PopulateRanking()
    {
        // --- VERIFICAÇÃO DE SEGURANÇA #1: RankingManager ---
        if (RankingManager.instance == null)
        {
            Debug.LogError("RankingManager não encontrado na cena! A UI de Ranking será desativada.", this.gameObject);
            gameObject.SetActive(false); // Desativa o objeto para evitar mais erros.
            return;
        }

        // Pega a lista de scores ordenada.
        List<ScoreEntry> ranking = RankingManager.instance.GetRanking();

        // --- VERIFICAÇÃO DE SEGURANÇA #2: Referências do Top 3 ---
        // Se as referências do Top 3 não estiverem atribuídas, apenas mostra um aviso e continua.
        if (top1_Text == null || top2_Text == null || top3_Text == null)
        {
            Debug.LogWarning("Uma ou mais referências de texto do Top 3 não foram atribuídas no Inspector.", this.gameObject);
        }
        else
        {
            top1_Text.text = ranking.Count > 0 ? $"1. {ranking[0].playerName} - {ranking[0].score}" : "1. ...";
            top2_Text.text = ranking.Count > 1 ? $"2. {ranking[1].playerName} - {ranking[1].score}" : "2. ...";
            top3_Text.text = ranking.Count > 2 ? $"3. {ranking[2].playerName} - {ranking[2].score}" : "3. ...";
        }

        // --- VERIFICAÇÃO DE SEGURANÇA #3: Referências dos Outros Ranks ---
        // Se o container ou o template não estiverem atribuídos, o código para aqui,
        // mas sem recarregar a cena.
        if (otherRanks_ContainerParent == null)
        {
            Debug.LogError("'otherRanks_ContainerParent' não está atribuído no Inspector! Não é possível popular o resto do ranking.", this.gameObject);
            return; // PARA A EXECUÇÃO DO MÉTODO AQUI.
        }
        if (otherRanks_TemplateText == null)
        {
            Debug.LogError("'otherRanks_TemplateText' não está atribuído no Inspector! Não é possível popular o resto do ranking.", this.gameObject);
            return; // PARA A EXECUÇÃO DO MÉTODO AQUI.
        }

        // Limpa o conteúdo antigo antes de adicionar o novo.
        foreach (Transform child in otherRanks_ContainerParent)
        {
            Destroy(child.gameObject);
        }

        // Ativa o template para o caso de ele estar desativado no editor.
        otherRanks_TemplateText.gameObject.SetActive(true);

        // Preenche os outros (do 4º ao 10º lugar).
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

        // Desativa o template original para que ele não apareça na lista final.
        otherRanks_TemplateText.gameObject.SetActive(false);
    }

    // A função GoToGame() foi REMOVIDA para centralizar o controle de cenas
    // no MenuManager e LevelButton, evitando o recarregamento indesejado da cena.
}