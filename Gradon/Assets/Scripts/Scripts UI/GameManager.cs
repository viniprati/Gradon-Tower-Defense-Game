// GameManager.cs (Vers�o Final com Gerenciamento de Mana)

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // A propriedade 'Instance' � a forma padr�o e segura de implementar um Singleton.
    public static GameManager Instance { get; private set; }

    [Header("Cat�logo de Fases")]
    [Tooltip("Esta lista � preenchida automaticamente da pasta 'Resources/Levels'.")]
    public List<LevelData> allLevels;

    [Header("Estado do Jogo")]
    public LevelData currentLevelData { get; private set; }
    private bool isGameOver = false;

    [Header("Recursos do Jogador")]
    [Tooltip("A quantidade atual de mana do jogador.")]
    public float currentMana { get; private set; }

    // --- Eventos para Desacoplar a L�gica ---
    // A UI e outros sistemas v�o "ouvir" esses eventos para saber quando agir.
    public static event Action<float> OnManaChanged; // Disparado quando a mana muda.
    public static event Action<bool> OnGameOver;    // Disparado no fim do jogo.

    void Awake()
    {
        // L�gica do Singleton para garantir que s� exista um GameManager.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Garante que o GameManager persista entre as cenas.
            LoadAllLevelsFromResources();
        }
        else
        {
            // Se j� existe uma inst�ncia, esta � duplicada e deve ser destru�da.
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
        // Se desinscreve para evitar erros quando o objeto for destru�do.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Chamado automaticamente sempre que uma nova cena � carregada.
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
    /// Prepara e carrega a cena de uma fase espec�fica.
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
    /// Define a mana inicial no come�o da fase e notifica a UI.
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
    /// <returns>Retorna 'true' se a mana foi gasta com sucesso, e 'false' se n�o havia mana suficiente.</returns>
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
            Debug.LogWarning("Tentativa de gastar " + amountToSpend + " de mana, mas s� tem " + currentMana);
            return false; // Falha
        }
    }

    /// <summary>
    /// Gerencia a l�gica de fim de jogo (vit�ria ou derrota).
    /// </summary>
    public void HandleGameOver(bool playerWon)
    {
        if (isGameOver) return; // Evita chamadas m�ltiplas
        isGameOver = true;

        OnGameOver?.Invoke(playerWon); // Notifica a UI para mostrar a tela de vit�ria/derrota
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