// GameManager.cs (Vers�o Completa com Carregamento Autom�tico)

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq; // Necess�rio para ordenar a lista (OrderBy)

public class GameManager : MonoBehaviour
{
    // AVISO: Seu c�digo usa 'instance' com 'i' min�sculo. 
    // Mantenho essa conven��o para compatibilidade.
    public static GameManager instance;

    [Header("Cat�logo de Fases")]
    // Esta lista agora � preenchida automaticamente pelo c�digo.
    public List<LevelData> allLevels;
    public LevelData currentLevelData { get; private set; }

    [Header("Estado do Jogo")]
    private bool isGameOver = false;

    [Header("Refer�ncias de UI")]
    public TextMeshProUGUI manaText;
    public GameObject gameOverPanel;

    public static event Action<float> OnManaChanged;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Chama o m�todo para carregar todas as fases da pasta "Resources"
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
        // 1. Carrega todos os assets do tipo LevelData. O "" significa "procure em toda a pasta Resources".
        LevelData[] loadedLevels = Resources.LoadAll<LevelData>("");

        // 2. Converte o array para uma lista.
        allLevels = new List<LevelData>(loadedLevels);

        // 3. (Opcional, mas recomendado) Ordena a lista pelo nome do arquivo, para que apare�am em ordem no menu.
        allLevels = allLevels.OrderBy(level => level.name).ToList();

        Debug.Log($"[GameManager] Carregados {allLevels.Count} n�veis da pasta Resources automaticamente.");
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// M�todo chamado pelo bot�o no menu para definir qual fase ser� jogada.
    /// Ele APENAS armazena a informa��o, n�o carrega a cena.
    /// </summary>
    public void SetSelectedLevel(LevelData levelData)
    {
        currentLevelData = levelData;
        Debug.Log($"Fase '{levelData.name}' selecionada. Pronto para carregar a cena.");
    }

    /// <summary>
    /// Carrega uma fase usando seu �ndice (ex: 1).
    /// Este m�todo ainda pode ser �til para outras coisas, ent�o vamos mant�-lo.
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
            Debug.LogError($"Nenhuma fase encontrada com o �ndice {levelIndex}!", this.gameObject);
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

    /// <summary>
    /// Controla o que acontece quando a fase termina (vit�ria ou derrota).
    /// </summary>
    public void HandleGameOver(bool playerWon)
    {
        if (isGameOver) return;
        isGameOver = true;

        if (playerWon)
        {
            StartCoroutine(VictorySequence());
        }
        else
        {
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