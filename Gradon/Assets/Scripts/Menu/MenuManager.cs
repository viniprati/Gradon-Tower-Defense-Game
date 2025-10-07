// MenuManager.cs (Corrigido)

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [Header("Referências da UI de Seleção")]
    [Tooltip("Arraste aqui o objeto de texto que exibirá o nome e o número da fase.")]
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
            Debug.LogError("GameManager não foi encontrado! A seleção de fases não funcionará.");
            if (levelInfoText != null) levelInfoText.text = "ERRO";
            return;
        }

        if (allLevels == null || allLevels.Count == 0)
        {
            if (levelInfoText != null) levelInfoText.text = "Nenhuma Fase Disponível";
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

    // --- MÉTODO CORRIGIDO AQUI ---
    /// <summary>
    /// Chamado pelo botão "Iniciar".
    /// </summary>
    public void StartGame() // Ou StartSelectedLevel()
    {
        if (allLevels.Count > 0 && GameManager.instance != null)
        {
            // 1. Pega o ARQUIVO DE DADOS completo da fase selecionada.
            LevelData levelToLoad = allLevels[selectedLevelIndex];

            // 2. Entrega o ARQUIVO DE DADOS (e não mais o número) para o GameManager.
            GameManager.instance.LoadLevel(levelToLoad);
        }
        else
        {
            Debug.LogError("Não foi possível iniciar a fase. Verifique a configuração do GameManager.", this.gameObject);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}