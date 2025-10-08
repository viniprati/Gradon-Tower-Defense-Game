// MenuManager.cs (Versão com textos separados para Número e Nome)

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

    [Tooltip("Arraste aqui o texto que exibirá o NOME da fase.")]
    [SerializeField] private TextMeshProUGUI levelNameText; // Renomeado

    [Tooltip("Arraste aqui o texto que exibirá o NÚMERO da fase.")]
    [SerializeField] private TextMeshProUGUI levelNumberText; // Novo

    [Tooltip("Arraste aqui o botão de INICIAR.")]
    [SerializeField] private Button startButton;

    [Tooltip("Arraste aqui o botão de SAIR.")]
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
            Debug.LogError("GameManager não foi encontrado!");
            if (levelNameText != null) levelNameText.text = "ERRO";
            SetButtonsInteractable(false);
            return;
        }

        if (allLevels == null || allLevels.Count == 0)
        {
            Debug.LogWarning("Nenhuma fase foi encontrada na lista do GameManager.");
            if (levelNameText != null) levelNameText.text = "Nenhuma Fase Disponível";
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

    // --- FUNÇÃO MODIFICADA ---
    private void UpdateLevelDisplay()
    {
        if (allLevels.Count == 0) return;

        LevelData selectedLevelData = allLevels[selectedLevelIndex];

        // Atualiza o texto do NOME
        if (levelNameText != null)
        {
            levelNameText.text = selectedLevelData.levelName;
        }

        // Atualiza o texto do NÚMERO
        if (levelNumberText != null)
        {
            // .ToString() converte o número para texto
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
            Debug.LogError("Não foi possível iniciar a fase.", this.gameObject);
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