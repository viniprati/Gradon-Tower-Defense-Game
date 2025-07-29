// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Use TextMeshPro para textos mais bonitos
using UnityEngine.UI;
using System;
using System.Collections; // Para a imagem do ícone da torre

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Estado do Jogo")]
    private int score = 0;
    private bool isGameOver = false;

    [Header("Recursos do Jogador")]
    private float currentMana = 0;

    [Header("Referências de UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI manaText; // Texto para exibir a mana/moedas
    public GameObject gameOverPanel;

    [Header("UI de Game Over e Ranking")]
    public GameObject enterNamePanel; // NOVO: Painel com o campo de input e botão submit
    public TMP_InputField nameInputField; // NOVO: O campo de texto para o nome
    public TextMeshProUGUI finalScoreText; // NOVO: Texto para mostrar o score final no painel

    // EVENTO: Qualquer script poderá "se inscrever" para ser notificado quando a mana mudar.
    public static event Action<float> OnManaChanged;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // Opcional
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateScoreUI();
    }

    public void AddScore(int points)
    {
        if (isGameOver) return;
        score += points;
        UpdateScoreUI();
    }

    // PlayerController vai chamar esta função
    public void UpdateManaUI(float newManaValue)
    {
        currentMana = newManaValue;
        if (manaText != null)
        {
            manaText.text = "Mana: " + Mathf.FloorToInt(currentMana).ToString();
        }

        // DISPARA O EVENTO: Notifica todos os inscritos sobre a nova quantidade de mana.
        OnManaChanged?.Invoke(currentMana);
    }


    // O método público que é chamado quando o jogador morre
    public void HandleGameOver()
    {
        // A única coisa que este método faz agora é iniciar a coroutine
        // e garantir que ela não seja chamada mais de uma vez.
        if (isGameOver) return;
        isGameOver = true;

        StartCoroutine(GameOverSequence());
    }

    // A Coroutine que controla a sequência de telas
    private IEnumerator GameOverSequence()
    {
        // --- ETAPA 1: Mostrar a tela "GAME OVER" ---

        // Pausa o jogo. Como estamos usando WaitForSecondsRealtime,
        // a pausa não afetará nosso tempo de espera.
        Time.timeScale = 0f;

        // Esconde o painel de inserir nome, caso ele estivesse visível por algum bug
        if (enterNamePanel != null) enterNamePanel.SetActive(false);

        // Ativa o painel de Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Painel de Game Over não foi atribuído no GameManager!");
        }

        // Espera por 2 segundos em tempo real (ignora o Time.timeScale = 0)
        yield return new WaitForSecondsRealtime(2f);


        // --- ETAPA 2: Trocar para a tela de "Inserir Nome" ---

        // Ativa o painel de inserir nome
        if (enterNamePanel != null)
        {
            enterNamePanel.SetActive(true);

            // Preenche o score final
            if (finalScoreText != null)
            {
                finalScoreText.text = "Seu Score: " + score.ToString();
            }
            else
            {
                Debug.LogWarning("Texto de Score Final não foi atribuído no GameManager!");
            }

            // Foca o campo de texto para o jogador começar a digitar
            if (nameInputField != null)
            {
                nameInputField.Select();
                nameInputField.ActivateInputField();
            }
            else
            {
                Debug.LogWarning("Campo de Input de Nome não foi atribuído no GameManager!");
            }
        }
        else
        {
            Debug.LogWarning("Painel de Inserir Nome não foi atribuído no GameManager!");
        }
    }
    // NOVO MÉTODO: Será chamado pelo botão "Submit" da UI
    public void SubmitScore()
    {
        if (nameInputField.text == "")
        {
            // Opcional: Mostrar um aviso de que o nome não pode ser vazio
            Debug.Log("Por favor, insira um nome.");
            return;
        }

        // Adiciona o score ao RankingManager
        if (RankingManager.instance != null)
        {
            RankingManager.instance.AddScore(nameInputField.text, score);
        }

        // Retorna ao estado normal do tempo e carrega a cena do menu
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene"); // !! IMPORTANTE: Mude "MenuScene" para o nome real da sua cena de menu
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}