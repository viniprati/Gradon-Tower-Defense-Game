// GameManager.cs (Vers�o Completa com Carregamento Autom�tico, L�gica de Game Over Corrigida e DEPURA��O)

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

    [Header("Cat�logo de Fases")]
    public List<LevelData> allLevels;
    public LevelData currentLevelData { get; private set; }

    [Header("Estado do Jogo")]
    private bool isGameOver = false;

    [Header("Refer�ncias de UI")]
    public TextMeshProUGUI manaText;
    public GameObject gameOverPanel;

    public static event Action<float> OnManaChanged;

    // =================================================================================
    // C�DIGO DE DEPURA��O ADICIONADO
    // =================================================================================
    void OnEnable()
    {
        // Se inscreve no evento que � chamado toda vez que uma cena � carregada
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Limpa a inscri��o para evitar erros
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"<color=orange>NOVA CENA CARREGADA: '{scene.name}'.</color>");
        // Checa se o objeto atual ainda � a inst�ncia principal do GameManager
        if (this == instance)
        {
            Debug.Log("<color=green>Eu (GameManager) sobrevivi � transi��o de cena!</color>");
            // Checa se os dados da fase ainda existem ap�s carregar a cena
            if (currentLevelData != null)
            {
                Debug.Log($"<color=green>DADOS DA FASE PERSISTIRAM! Fase selecionada: '{currentLevelData.name}'.</color>");
            }
            else
            {
                Debug.LogError("[GameManager] ERRO CR�TICO: Sobrevivi � transi��o, mas os dados da fase (currentLevelData) foram perdidos!");
            }
        }
    }
    // =================================================================================

    void Awake()
    {
        Debug.Log($"[GameManager] AWAKE na cena '{SceneManager.GetActiveScene().name}'. GameObject: '{this.gameObject.name}'");

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("<color=green>[GameManager] Inst�ncia definida e marcada como DontDestroyOnLoad.</color>");

            LoadAllLevelsFromResources();
        }
        else if (instance != this)
        {
            Debug.LogWarning($"[GameManager] Inst�ncia duplicada encontrada. Destruindo este objeto '{this.gameObject.name}'.");
            Destroy(gameObject);
        }
    }

    private void LoadAllLevelsFromResources()
    {
        LevelData[] loadedLevels = Resources.LoadAll<LevelData>("");
        allLevels = new List<LevelData>(loadedLevels);
        allLevels = allLevels.OrderBy(level => level.name).ToList();
        Debug.Log($"[GameManager] Carregados {allLevels.Count} n�veis da pasta Resources automaticamente.");
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void SetSelectedLevel(LevelData levelData)
    {
        currentLevelData = levelData;
        Debug.Log($"Fase '{levelData.name}' selecionada. Pronto para carregar a cena.");
    }

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
            Debug.LogError($"Nenhuma fase encontrada com o �ndice {levelIndex}!", this.gameObject);
        }
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
            Debug.Log("CONDI��O DE VIT�RIA ATINGIDA!");
            StartCoroutine(VictorySequence());
        }
        else
        {
            Debug.Log("CONDI��O DE DERROTA ATINGIDA!");
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
            Debug.Log("<color=green>FASE CONCLU�DA!</color>");
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