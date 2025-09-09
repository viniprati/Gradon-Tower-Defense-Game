// GameManager.cs (Atualizado com Sistema de Fases)

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Use TextMeshPro para textos mais bonitos
using System.Collections.Generic; // Para a lista de fases
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // --- NOVA SE��O: CONTROLE DE FASES ---
    [Header("Cat�logo de Fases do Jogo")]
    [Tooltip("Arraste todos os seus arquivos de fase (LevelData) para esta lista.")]
    public List<LevelData> allLevels;

    // Guarda os dados da fase que est� sendo jogada no momento.
    public LevelData currentLevelData { get; private set; }


    [Header("Estado do Jogo")]
    private int score = 0;
    private bool isGameOver = false;


    [Header("Recursos do Jogador")]
    // A mana agora ser� controlada pelo Totem na cena de jogo,
    // mas o GameManager pode ajudar a atualizar a UI se necess�rio.
    private float currentMana = 0;


    [Header("Refer�ncias de UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI manaText;
    public GameObject gameOverPanel;
    public GameObject enterNamePanel;
    public TMP_InputField nameInputField;
    public TextMeshProUGUI finalScoreText;

    // Evento para notificar a UI sobre mudan�as na mana.
    public static event Action<float> OnManaChanged;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Essencial para o sistema de fases funcionar entre as cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (enterNamePanel != null) enterNamePanel.SetActive(false);
        UpdateScoreUI();
    }

    // --- NOVOS M�TODOS PARA CARREGAR FASES ---

    /// <summary>
    /// Carrega uma fase usando seu �ndice (ex: 1). Perfeito para bot�es de menu.
    /// </summary>
    public void LoadLevel(int levelIndex)
    {
        // Procura na nossa lista por uma fase com o �ndice correspondente.
        LevelData levelToLoad = allLevels.Find(level => level.levelIndex == levelIndex);

        if (levelToLoad != null)
        {
            // Guarda os dados da fase que queremos carregar
            currentLevelData = levelToLoad;

            // Carrega a cena/mapa associada a esta fase
            SceneManager.LoadScene(currentLevelData.sceneToLoad);
        }
        else
        {
            Debug.LogError($"Nenhuma fase encontrada com o �ndice {levelIndex}! Verifique o cat�logo de fases no GameManager.");
        }
    }


    // --- M�TODOS EXISTENTES ---

    public void AddScore(int points)
    {
        if (isGameOver) return;
        score += points;
        UpdateScoreUI();
    }

    public void UpdateManaUI(float newManaValue)
    {
        currentMana = newManaValue;
        if (manaText != null)
        {
            manaText.text = "Mana: " + Mathf.FloorToInt(currentMana).ToString();
        }
        OnManaChanged?.Invoke(currentMana);
    }

    public void HandleGameOver(bool playerWon)
    {
        if (isGameOver) return;
        isGameOver = true;

        if (playerWon)
        {
            // Se o jogador venceu, vai direto para a tela de inserir nome/score.
            StartCoroutine(VictorySequence());
        }
        else
        {
            // Se perdeu, mostra a tela de "Game Over" primeiro.
            StartCoroutine(DefeatSequence());
        }
    }

    private IEnumerator DefeatSequence()
    {
        Time.timeScale = 0f;
        if (enterNamePanel != null) enterNamePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        // Depois de mostrar "Game Over", voc� pode decidir o que fazer:
        // - Ir para a tela de Score (como na vit�ria)
        // - Ou simplesmente carregar o menu
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene"); // Exemplo
    }

    private IEnumerator VictorySequence()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (enterNamePanel != null)
        {
            enterNamePanel.SetActive(true);
            if (finalScoreText != null) finalScoreText.text = "Seu Score: " + score.ToString();
            if (nameInputField != null)
            {
                nameInputField.Select();
                nameInputField.ActivateInputField();
            }
        }
        yield return null;
    }

    public void SubmitScore()
    {
        if (nameInputField.text == "") return;

        // Adiciona o score ao RankingManager (se existir)
        // RankingManager.instance.AddScore(nameInputField.text, score);

        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        // Recarrega a fase atual
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

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void HandleGameOver()
    {

    }
}