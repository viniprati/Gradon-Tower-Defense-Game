// GameManager.cs (Versão Final com Gerenciamento de Mana)

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // A propriedade 'Instance' é a forma padrão e segura de implementar um Singleton.
    public static GameManager Instance { get; private set; }

    [Header("Catálogo de Fases")]
    [Tooltip("Esta lista é preenchida automaticamente da pasta 'Resources/Levels'.")]
    public List<LevelData> allLevels;

    [Header("Estado do Jogo")]
    public LevelData currentLevelData { get; private set; }
    private bool isGameOver = false;

    [Header("Recursos do Jogador")]
    [Tooltip("A quantidade atual de mana do jogador.")]
    public float currentMana { get; private set; }

    // --- Eventos para Desacoplar a Lógica ---
    // A UI e outros sistemas vão "ouvir" esses eventos para saber quando agir.
    public static event Action<float> OnManaChanged; // Disparado quando a mana muda.
    public static event Action<bool> OnGameOver;    // Disparado no fim do jogo.

    void Awake()
    {
        // Lógica do Singleton para garantir que só exista um GameManager.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Garante que o GameManager persista entre as cenas.
            LoadAllLevelsFromResources();
        }
        else
        {
            // Se já existe uma instância, esta é duplicada e deve ser destruída.
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        // Se inscreve no evento de carregamento de cena para poder inicializar a mana.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Se desinscreve para evitar erros quando o objeto for destruído.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Chamado automaticamente sempre que uma nova cena é carregada.
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"CENA CARREGADA: '{scene.name}'.");

        // Se carregamos uma cena que corresponde a uma fase, inicializa a mana.
        if (currentLevelData != null && scene.name == currentLevelData.sceneToLoad)
        {
            InitializeMana(currentLevelData.startingMana);
        }
    }

    /// <summary>
    /// Carrega todos os ScriptableObjects de LevelData da pasta Resources/Levels.
    /// </summary>
    private void LoadAllLevelsFromResources()
    {
        LevelData[] loadedLevels = Resources.LoadAll<LevelData>("Levels");
        allLevels = new List<LevelData>(loadedLevels.OrderBy(level => level.levelIndex));
        Debug.Log($"[GameManager] {allLevels.Count} fases foram carregadas.");
    }

    /// <summary>
    /// Prepara e carrega a cena de uma fase específica.
    /// </summary>
    public void LoadLevel(LevelData levelToLoad)
    {
        if (levelToLoad == null)
        {
            Debug.LogError("Tentativa de carregar uma fase nula (LevelData)!");
            return;
        }
        this.currentLevelData = levelToLoad;
        isGameOver = false;
        SceneManager.LoadScene(levelToLoad.sceneToLoad);
    }

    /// <summary>
    /// Define a mana inicial no começo da fase e notifica a UI.
    /// </summary>
    private void InitializeMana(float startingAmount)
    {
        currentMana = startingAmount;
        OnManaChanged?.Invoke(currentMana); // Dispara o evento para atualizar a UI
        Debug.Log($"<color=lightblue>Mana da fase inicializada com {currentMana}.</color>");
    }

    /// <summary>
    /// Adiciona mana ao total do jogador e notifica a UI.
    /// </summary>
    public void AddMana(float amountToAdd)
    {
        currentMana += amountToAdd;
        OnManaChanged?.Invoke(currentMana); // Dispara o evento para atualizar a UI
    }

    /// <summary>
    /// Tenta gastar uma quantidade de mana.
    /// </summary>
    /// <returns>Retorna 'true' se a mana foi gasta com sucesso, e 'false' se não havia mana suficiente.</returns>
    public bool SpendMana(float amountToSpend)
    {
        if (currentMana >= amountToSpend)
        {
            currentMana -= amountToSpend;
            OnManaChanged?.Invoke(currentMana); // Dispara o evento para atualizar a UI
            return true; // Sucesso
        }
        else
        {
            // Opcional: Adicionar um som de "erro" ou feedback visual aqui.
            Debug.LogWarning("Tentativa de gastar " + amountToSpend + " de mana, mas só tem " + currentMana);
            return false; // Falha
        }
    }

    /// <summary>
    /// Gerencia a lógica de fim de jogo (vitória ou derrota).
    /// </summary>
    public void HandleGameOver(bool playerWon)
    {
        if (isGameOver) return; // Evita chamadas múltiplas
        isGameOver = true;

        OnGameOver?.Invoke(playerWon); // Notifica a UI para mostrar a tela de vitória/derrota
        StartCoroutine(EndSequence());
    }

    /// <summary>
    /// Pausa o jogo, espera um tempo e volta para o menu.
    /// </summary>
    private IEnumerator EndSequence()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3f); // Espera 3s em tempo real (ignora o timeScale = 0)
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene"); // Mude "MenuScene" para o nome exato da sua cena de menu
    }

    /// <summary>
    /// Reinicia a fase atual.
    /// </summary>
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