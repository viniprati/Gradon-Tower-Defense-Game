// GameManager.cs (Versão Completa com Carregamento Automático e Lógica de Game Over Corrigida)

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq; // Necessário para ordenar a lista (OrderBy)

public class GameManager : MonoBehaviour
{
    // AVISO: Seu código usa 'instance' com 'i' minúsculo. 
    // Mantenho essa convenção para compatibilidade.
    public static GameManager instance;

    [Header("Catálogo de Fases")]
    // Esta lista agora é preenchida automaticamente pelo código.
    public List<LevelData> allLevels;
    public LevelData currentLevelData { get; private set; }

    [Header("Estado do Jogo")]
    private bool isGameOver = false;

    [Header("Referências de UI")]
    public TextMeshProUGUI manaText;
    public GameObject gameOverPanel;

    public static event Action<float> OnManaChanged;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAllLevelsFromResources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Encontra e carrega todos os ScriptableObjects do tipo LevelData
    /// que estiverem dentro de qualquer pasta chamada "Resources" no projeto.
    /// </summary>
    private void LoadAllLevelsFromResources()
    {
        LevelData[] loadedLevels = Resources.LoadAll<LevelData>("");
        allLevels = new List<LevelData>(loadedLevels);
        allLevels = allLevels.OrderBy(level => level.name).ToList();
        Debug.Log($"[GameManager] Carregados {allLevels.Count} níveis da pasta Resources automaticamente.");
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// Método chamado pelo botão no menu para definir qual fase será jogada.
    /// </summary>
    public void SetSelectedLevel(LevelData levelData)
    {
        currentLevelData = levelData;
        Debug.Log($"Fase '{levelData.name}' selecionada. Pronto para carregar a cena.");
    }

    /// <summary>
    /// Carrega uma fase usando seu índice (ex: 1).
    /// </summary>
    public void LoadLevel(int levelIndex)
    {
        LevelData levelToLoad = allLevels.Find(level => level.levelIndex == levelIndex);

        if (levelToLoad != null)
        {
            currentLevelData = levelToLoad;
            SceneManager.LoadScene(currentLevelData.sceneToLoad);
        }
        else
        {
            Debug.LogError($"Nenhuma fase encontrada com o índice {levelIndex}!", this.gameObject);
        }
    }

    /// <summary>
    /// Chamado pelo Totem para atualizar a UI de mana.
    /// </summary>
    public void UpdateManaUI(float newManaValue)
    {
        if (manaText != null)
        {
            manaText.text = "Mana: " + Mathf.FloorToInt(newManaValue).ToString();
        }
        OnManaChanged?.Invoke(newManaValue);
    }

    // =================================================================================
    // MÉTODO MODIFICADO PARA CORRIGIR A CONDIÇÃO DE CORRIDA
    // =================================================================================
    /// <summary>
    /// Controla o que acontece quando a fase termina (vitória ou derrota).
    /// Esta versão prioriza a derrota caso ocorra ao mesmo tempo que a vitória.
    /// </summary>
    public void HandleGameOver(bool playerWon)
    {
        // Se o jogo já acabou E a nova chamada não for uma derrota, ignore.
        // Isso permite que uma derrota (playerWon = false) SOBRESCREVA uma vitória.
        if (isGameOver && playerWon)
        {
            return;
        }

        // Se o jogo não acabou ou se a nova chamada é uma derrota, continue.
        isGameOver = true;
        StopAllCoroutines(); // Para a sequência de vitória se uma derrota acontecer.

        if (playerWon)
        {
            Debug.Log("CONDIÇÃO DE VITÓRIA ATINGIDA!");
            StartCoroutine(VictorySequence());
        }
        else
        {
            Debug.Log("CONDIÇÃO DE DERROTA ATINGIDA!");
            StartCoroutine(DefeatSequence());
        }
    }
    // =================================================================================

    private IEnumerator DefeatSequence()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    private IEnumerator VictorySequence()
    {
        if (gameOverPanel != null)
        {
            Debug.Log("<color=green>FASE CONCLUÍDA!</color>");
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        if (currentLevelData != null)
        {
            SceneManager.LoadScene(currentLevelData.sceneToLoad);
        }
        else
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
}