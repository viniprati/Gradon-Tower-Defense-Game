// MenuManager.cs (Corrigido)

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [Header("Refer�ncias da UI de Sele��o")]
    [Tooltip("Arraste aqui o objeto de texto que exibir� o nome e o n�mero da fase.")]
    [SerializeField] private TextMeshProUGUI levelInfoText;

    private int selectedLevelIndex = 0;
    private List<LevelData> allLevels;

    void Start()
    {
        if (GameManager.instance != null)
        {
            allLevels = GameManager.instance.allLevels;
        }
        else
        {
            Debug.LogError("GameManager n�o foi encontrado! A sele��o de fases n�o funcionar�.");
            if (levelInfoText != null) levelInfoText.text = "ERRO";
            return;
        }

        if (allLevels == null || allLevels.Count == 0)
        {
            if (levelInfoText != null) levelInfoText.text = "Nenhuma Fase Dispon�vel";
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

    private void UpdateLevelDisplay()
    {
        LevelData selectedLevelData = allLevels[selectedLevelIndex];
        if (levelInfoText != null)
        {
            levelInfoText.text = $"{selectedLevelData.levelIndex}. {selectedLevelData.levelName}";
        }
    }

    // --- M�TODO CORRIGIDO AQUI ---
    /// <summary>
    /// Chamado pelo bot�o "Iniciar".
    /// </summary>
    public void StartGame() // Ou StartSelectedLevel()
    {
        if (allLevels.Count > 0 && GameManager.instance != null)
        {
            // 1. Pega o ARQUIVO DE DADOS completo da fase selecionada.
            LevelData levelToLoad = allLevels[selectedLevelIndex];

            // 2. Entrega o ARQUIVO DE DADOS (e n�o mais o n�mero) para o GameManager.
            GameManager.instance.LoadLevel(levelToLoad);
        }
        else
        {
            Debug.LogError("N�o foi poss�vel iniciar a fase. Verifique a configura��o do GameManager.", this.gameObject);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}