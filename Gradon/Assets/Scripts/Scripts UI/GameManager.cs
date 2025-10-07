// GameManager.cs

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Catálogo de Fases")]
    public List<LevelData> allLevels;
    public LevelData currentLevelData { get; private set; }

    [Header("Estado do Jogo")]
    private bool isGameOver = false;

    [Header("Referências de UI")]
    public TextMeshProUGUI manaText;
    public GameObject gameOverPanel;

    public static event Action<float> OnManaChanged;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"<color=orange>NOVA CENA CARREGADA: '{scene.name}'.</color>");
        if (this == instance)
        {
            Debug.Log("<color=green>Eu (GameManager) sobrevivi à transição de cena!</color>");
            if (currentLevelData != null)
            {
                Debug.Log($"<color=green>DADOS DA FASE PERSISTIRAM! Fase selecionada: '{currentLevelData.levelName}'.</color>");
            }
            else
            {
                Debug.LogError("[GameManager] ERRO CRÍTICO: Sobrevivi à transição, mas os dados da fase (currentLevelData) foram perdidos!");
            }
        }
    }

    void Awake()
    {
        Debug.Log($"[GameManager] AWAKE na cena '{SceneManager.GetActiveScene().name}'. GameObject: '{this.gameObject.name}'");

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("<color=green>[GameManager] Instância definida e marcada como DontDestroyOnLoad.</color>");
            LoadAllLevelsFromResources();
        }
        else if (instance != this)
        {
            Debug.LogWarning($"[GameManager] Instância duplicada encontrada. Destruindo este objeto '{this.gameObject.name}'.");
            Destroy(gameObject);
        }
    }

    private void LoadAllLevelsFromResources()
    {
        LevelData[] loadedLevels = Resources.LoadAll<LevelData>("Levels"); // Procura na pasta Resources/Levels
        allLevels = new List<LevelData>(loadedLevels);
        allLevels = allLevels.OrderBy(level => level.levelIndex).ToList();
        Debug.Log($"[GameManager] Carregados {allLevels.Count} níveis da pasta Resources automaticamente.");
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// O método principal para carregar uma fase. Recebe os dados completos do MenuManager.
    /// </summary>
    public void LoadLevel(LevelData levelToLoad)
    {
        if (levelToLoad == null)
        {
            Debug.LogError("Tentativa de carregar uma fase, mas os dados da fase (LevelData) são nulos!");
            return;
        }

        this.currentLevelData = levelToLoad;
        Debug.Log($"<color=cyan>GameManager está pronto para carregar: {currentLevelData.levelName}</color>");

        isGameOver = false; // Reseta o estado de 'game over' para a nova fase

        SceneManager.LoadScene(currentLevelData.sceneToLoad);
    }

    public void UpdateManaUI(float newManaValue)
    {
        if (manaText != null)
        {
            manaText.text = "Mana: " + Mathf.FloorToInt(newManaValue).ToString();
        }
        OnManaChanged?.Invoke(newManaValue);
    }

    public void HandleGameOver(bool playerWon)
    {
        if (isGameOver && playerWon)
        {
            return;
        }

        isGameOver = true;
        StopAllCoroutines();

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