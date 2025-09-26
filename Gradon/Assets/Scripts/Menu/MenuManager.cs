// MenuManager.cs

using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Refer�ncias dos Pain�is")]
    [Tooltip("Arraste o painel do menu principal aqui.")]
    [SerializeField] private GameObject mainMenuPanel;

    [Tooltip("Arraste o painel de sele��o de fases aqui.")]
    [SerializeField] private GameObject levelSelectPanel;

    // O m�todo Start � chamado quando a cena carrega
    void Start()
    {
        // Garante que o estado inicial est� sempre correto
        // Mostra o menu principal e esconde a sele��o de fases
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    /// <summary>
    /// Esta � a fun��o que o bot�o "Iniciar" vai chamar.
    /// Ela esconde o menu principal e mostra o painel de sele��o de fases.
    /// </summary>
    public void ShowLevelSelectPanel()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    // Voc� pode adicionar outras fun��es aqui, como um bot�o para sair do jogo
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}