// MenuManager.cs

using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Referências dos Painéis")]
    [Tooltip("Arraste o painel do menu principal aqui.")]
    [SerializeField] private GameObject mainMenuPanel;

    [Tooltip("Arraste o painel de seleção de fases aqui.")]
    [SerializeField] private GameObject levelSelectPanel;

    // O método Start é chamado quando a cena carrega
    void Start()
    {
        // Garante que o estado inicial está sempre correto
        // Mostra o menu principal e esconde a seleção de fases
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    /// <summary>
    /// Esta é a função que o botão "Iniciar" vai chamar.
    /// Ela esconde o menu principal e mostra o painel de seleção de fases.
    /// </summary>
    public void ShowLevelSelectPanel()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    // Você pode adicionar outras funções aqui, como um botão para sair do jogo
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}