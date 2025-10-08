// MenuManager.cs (Vers�o Final com Textos Separados para N�mero e Nome)

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [Header("Refer�ncias da UI de Sele��o")]
    [Tooltip("Arraste aqui o bot�o da seta DIREITA.")]
    [SerializeField] private Button nextButton;

    [Tooltip("Arraste aqui o bot�o da seta ESQUERDA.")]
    [SerializeField] private Button previousButton;

    [Tooltip("Arraste aqui o texto que exibir� o NOME da fase.")]
    [SerializeField] private TextMeshProUGUI levelNameText;

    [Tooltip("Arraste aqui o texto que exibir� o N�MERO da fase.")]
    [SerializeField] private TextMeshProUGUI levelNumberText;

    [Tooltip("Arraste aqui o bot�o de INICIAR.")]
    [SerializeField] private Button startButton;

    [Tooltip("Arraste aqui o bot�o de SAIR.")]
    [SerializeField] private Button quitButton;

    private int selectedLevelIndex = 0;
    private List<LevelData> allLevels;

    private void Awake()
    {
        // Configura os m�todos que ser�o chamados quando cada bot�o for clicado.
        if (nextButton != null) nextButton.onClick.AddListener(NextLevel);
        if (previousButton != null) previousButton.onClick.AddListener(PreviousLevel);
        if (startButton != null) startButton.onClick.AddListener(StartGame);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
    }

    void Start()
    {
        // Pega a lista de fases do GameManager.
        if (GameManager.Instance != null)
        {
            allLevels = GameManager.Instance.allLevels;
        }
        else
        {
            Debug.LogError("GameManager n�o foi encontrado! A sele��o de fases n�o funcionar�.");
            if (levelNameText != null) levelNameText.text = "ERRO";
            if (levelNumberText != null) levelNumberText.text = "!";
            SetButtonsInteractable(false);
            return;
        }

        // Verifica se existem fases cadastradas no GameManager.
        if (allLevels == null || allLevels.Count == 0)
        {
            Debug.LogWarning("Nenhuma fase foi encontrada na lista do GameManager.");
            if (levelNameText != null) levelNameText.text = "Nenhuma Fase Dispon�vel";
            if (levelNumberText != null) levelNumberText.text = "0";
            SetButtonsInteractable(false);
            return;
        }

        // Se tudo estiver certo, exibe a primeira fase.
        UpdateLevelDisplay();
    }

    // Navega para a pr�xima fase na lista (com loop)
    public void NextLevel()
    {
        if (allLevels.Count == 0) return;
        selectedLevelIndex = (selectedLevelIndex + 1) % allLevels.Count;
        UpdateLevelDisplay();
    }

    // Navega para a fase anterior na lista (com loop)
    public void PreviousLevel()
    {
        if (allLevels.Count == 0) return;
        selectedLevelIndex--;
        if (selectedLevelIndex < 0)
        {
            selectedLevelIndex = allLevels.Count - 1;
        }
        UpdateLevelDisplay();
    }

    // Atualiza a UI com as informa��es da fase selecionada
    private void UpdateLevelDisplay()
    {
        if (allLevels.Count == 0) return;

        LevelData selectedLevelData = allLevels[selectedLevelIndex];

        // Atualiza o texto do NOME
        if (levelNameText != null)
        {
            levelNameText.text = selectedLevelData.levelName;
        }

        // Atualiza o texto do N�MERO
        if (levelNumberText != null)
        {
            // .ToString() converte o n�mero (int) para texto (string)
            levelNumberText.text = selectedLevelData.levelIndex.ToString();
        }
    }

    /// <summary>
    /// Chamado pelo bot�o "Iniciar". Informa ao GameManager qual fase carregar e inicia o jogo.
    /// </summary>
    public void StartGame()
    {
        if (allLevels.Count > 0 && GameManager.Instance != null)
        {
            LevelData levelToLoad = allLevels[selectedLevelIndex];
            GameManager.Instance.LoadLevel(levelToLoad);
        }
        else
        {
            Debug.LogError("N�o foi poss�vel iniciar a fase. Verifique a configura��o do GameManager e se h� fases na lista.", this.gameObject);
        }
    }

    /// <summary>
    /// Chamado pelo bot�o "Sair". Fecha a aplica��o.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Fun��o auxiliar para ativar/desativar todos os bot�es de uma vez
    private void SetButtonsInteractable(bool isInteractable)
    {
        if (nextButton != null) nextButton.interactable = isInteractable;
        if (previousButton != null) previousButton.interactable = isInteractable;
        if (startButton != null) startButton.interactable = isInteractable;
    }

    // Remove os listeners quando o objeto for destru�do para evitar memory leaks.
    private void OnDestroy()
    {
        if (nextButton != null) nextButton.onClick.RemoveListener(NextLevel);
        if (previousButton != null) previousButton.onClick.RemoveListener(PreviousLevel);
        if (startButton != null) startButton.onClick.RemoveListener(StartGame);
        if (quitButton != null) quitButton.onClick.RemoveListener(QuitGame);
    }
}