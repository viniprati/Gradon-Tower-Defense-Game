// MenuManager.cs (Versão Final e Completa)

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [Header("Referências da UI de Seleção")]
    [Tooltip("Arraste aqui o botão da seta DIREITA.")]
    [SerializeField] private Button nextButton;

    [Tooltip("Arraste aqui o botão da seta ESQUERDA.")]
    [SerializeField] private Button previousButton;

    [Tooltip("Arraste aqui o objeto de texto que exibirá o nome e o número da fase.")]
    [SerializeField] private TextMeshProUGUI levelInfoText;

    [Tooltip("Arraste aqui o botão de INICIAR.")]
    [SerializeField] private Button startButton;

    [Tooltip("Arraste aqui o botão de SAIR.")]
    [SerializeField] private Button quitButton;


    private int selectedLevelIndex = 0;
    private List<LevelData> allLevels;

    // Usamos Awake para garantir que os listeners sejam configurados antes de qualquer outra coisa.
    private void Awake()
    {
        // Configura os métodos que serão chamados quando cada botão for clicado.
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
            Debug.LogError("GameManager não foi encontrado! A seleção de fases não funcionará.");
            if (levelInfoText != null) levelInfoText.text = "ERRO";
            SetButtonsInteractable(false);
            return;
        }

        // Verifica se existem fases cadastradas no GameManager.
        if (allLevels == null || allLevels.Count == 0)
        {
            Debug.LogWarning("Nenhuma fase foi encontrada na lista do GameManager.");
            if (levelInfoText != null) levelInfoText.text = "Nenhuma Fase Disponível";
            SetButtonsInteractable(false);
            return;
        }

        // Se tudo estiver certo, exibe a primeira fase.
        UpdateLevelDisplay();
    }

    // Navega para a próxima fase na lista (com loop)
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

    // Atualiza o texto da UI com as informações da fase selecionada
    private void UpdateLevelDisplay()
    {
        if (levelInfoText != null)
        {
            LevelData selectedLevelData = allLevels[selectedLevelIndex];
            levelInfoText.text = $"{selectedLevelData.levelIndex}. {selectedLevelData.levelName}";
        }
    }

    /// <summary>
    /// Chamado pelo botão "Iniciar". Informa ao GameManager qual fase carregar e inicia o jogo.
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
            Debug.LogError("Não foi possível iniciar a fase. Verifique a configuração do GameManager e se há fases na lista.", this.gameObject);
        }
    }

    /// <summary>
    /// Chamado pelo botão "Sair". Fecha a aplicação.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Função auxiliar para ativar/desativar todos os botões de uma vez
    private void SetButtonsInteractable(bool isInteractable)
    {
        if (nextButton != null) nextButton.interactable = isInteractable;
        if (previousButton != null) previousButton.interactable = isInteractable;
        if (startButton != null) startButton.interactable = isInteractable;
    }

    // Remove os listeners quando o objeto for destruído para evitar memory leaks.
    private void OnDestroy()
    {
        if (nextButton != null) nextButton.onClick.RemoveListener(NextLevel);
        if (previousButton != null) previousButton.onClick.RemoveListener(PreviousLevel);
        if (startButton != null) startButton.onClick.RemoveListener(StartGame);
        if (quitButton != null) quitButton.onClick.RemoveListener(QuitGame);
    }
}