// MenuManager.cs (Vers�o com textos separados para N�mero e Nome)

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
    [SerializeField] private TextMeshProUGUI levelNameText; // Renomeado

    [Tooltip("Arraste aqui o texto que exibir� o N�MERO da fase.")]
    [SerializeField] private TextMeshProUGUI levelNumberText; // Novo

    [Tooltip("Arraste aqui o bot�o de INICIAR.")]
    [SerializeField] private Button startButton;

    [Tooltip("Arraste aqui o bot�o de SAIR.")]
    [SerializeField] private Button quitButton;

    private int selectedLevelIndex = 0;
    private List<LevelData> allLevels;

    private void Awake()
    {
        if (nextButton != null) nextButton.onClick.AddListener(NextLevel);
        if (previousButton != null) previousButton.onClick.AddListener(PreviousLevel);
        if (startButton != null) startButton.onClick.AddListener(StartGame);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            allLevels = GameManager.Instance.allLevels;
        }
        else
        {
            Debug.LogError("GameManager n�o foi encontrado!");
            if (levelNameText != null) levelNameText.text = "ERRO";
            SetButtonsInteractable(false);
            return;
        }

        if (allLevels == null || allLevels.Count == 0)
        {
            Debug.LogWarning("Nenhuma fase foi encontrada na lista do GameManager.");
            if (levelNameText != null) levelNameText.text = "Nenhuma Fase Dispon�vel";
            SetButtonsInteractable(false);
            return;
        }

        UpdateLevelDisplay();
    }

    public void NextLevel()
    {
        if (allLevels.Count == 0) return;
        selectedLevelIndex = (selectedLevelIndex + 1) % allLevels.Count;
        UpdateLevelDisplay();
    }

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

    // --- FUN��O MODIFICADA ---
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
            // .ToString() converte o n�mero para texto
            levelNumberText.text = selectedLevelData.levelIndex.ToString();
        }
    }

    public void StartGame()
    {
        if (allLevels.Count > 0 && GameManager.Instance != null)
        {
            LevelData levelToLoad = allLevels[selectedLevelIndex];
            GameManager.Instance.LoadLevel(levelToLoad);
        }
        else
        {
            Debug.LogError("N�o foi poss�vel iniciar a fase.", this.gameObject);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void SetButtonsInteractable(bool isInteractable)
    {
        if (nextButton != null) nextButton.interactable = isInteractable;
        if (previousButton != null) previousButton.interactable = isInteractable;
        if (startButton != null) startButton.interactable = isInteractable;
    }

    private void OnDestroy()
    {
        if (nextButton != null) nextButton.onClick.RemoveListener(NextLevel);
        if (previousButton != null) previousButton.onClick.RemoveListener(PreviousLevel);
        if (startButton != null) startButton.onClick.RemoveListener(StartGame);
        if (quitButton != null) quitButton.onClick.RemoveListener(QuitGame);
    }
}