// GameManager.cs (Simplificado - Sem Score)

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Cat�logo de Fases")]
    [Tooltip("Arraste todos os seus arquivos de fase (LevelData) para esta lista.")]
    public List<LevelData> allLevels;
    public LevelData currentLevelData { get; private set; }

    [Header("Estado do Jogo")]
    private bool isGameOver = false;

    [Header("Refer�ncias de UI")]
    public TextMeshProUGUI manaText;
    public GameObject gameOverPanel; // Painel para a mensagem de "Game Over"

    // Evento para notificar a UI sobre mudan�as na mana.
    public static event Action<float> OnManaChanged;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Garante que o painel de Game Over comece desativado
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// Carrega uma fase usando seu �ndice (ex: 1).
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
        // A vari�vel 'currentMana' foi removida pois o Totem j� a gerencia.
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
        // Mostra a tela de "Game Over"
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // Pausa o jogo
        Time.timeScale = 0f;

        // Espera 3 segundos em tempo real
        yield return new WaitForSecondsRealtime(3f);

        // Volta ao normal e carrega o menu
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene"); // Mude para o nome da sua cena de menu
    }

    private IEnumerator VictorySequence()
    {
        // Mostra uma mensagem de vit�ria (pode ser no mesmo painel de Game Over)
        if (gameOverPanel != null)
        {
            // Voc� pode adicionar um texto dentro do painel para customizar a mensagem
            Debug.Log("<color=green>FASE CONCLU�DA!</color>");
            gameOverPanel.SetActive(true);
        }

        // Pausa o jogo
        Time.timeScale = 0f;

        // Espera 3 segundos e carrega o menu
        yield return new WaitForSecondsRealtime(3f);

        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene"); // Mude para o nome da sua cena de menu
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
            // Se n�o houver fase carregada, volta para o menu
            SceneManager.LoadScene("MenuScene");
        }
    }
}