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

    // --- NOVA SEÇÃO: CONTROLE DE FASES ---
    [Header("Catálogo de Fases do Jogo")]
    [Tooltip("Arraste todos os seus arquivos de fase (LevelData) para esta lista.")]
    public List<LevelData> allLevels;

    // Guarda os dados da fase que está sendo jogada no momento.
    public LevelData currentLevelData { get; private set; }


    [Header("Estado do Jogo")]
    private int score = 0;
    private bool isGameOver = false;


    [Header("Recursos do Jogador")]
    // A mana agora será controlada pelo Totem na cena de jogo,
    // mas o GameManager pode ajudar a atualizar a UI se necessário.
    private float currentMana = 0;


    [Header("Referências de UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI manaText;
    public GameObject gameOverPanel;
    public GameObject enterNamePanel;
    public TMP_InputField nameInputField;
    public TextMeshProUGUI finalScoreText;

    // Evento para notificar a UI sobre mudanças na mana.
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

    // --- NOVOS MÉTODOS PARA CARREGAR FASES ---

    /// <summary>
    /// Carrega uma fase usando seu índice (ex: 1). Perfeito para botões de menu.
    /// </summary>
    public void LoadLevel(int levelIndex)
    {
        // Procura na nossa lista por uma fase com o índice correspondente.
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
            Debug.LogError($"Nenhuma fase encontrada com o índice {levelIndex}! Verifique o catálogo de fases no GameManager.");
        }
    }


    // --- MÉTODOS EXISTENTES ---

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

        // Depois de mostrar "Game Over", você pode decidir o que fazer:
        // - Ir para a tela de Score (como na vitória)
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
            // Se não houver fase carregada, volta para o menu
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