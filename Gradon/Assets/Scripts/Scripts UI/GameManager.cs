// GameManager.cs (Versão Final e Completa)

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // --- MELHORIA 1: SINGLETON PADRÃO C# ---
    // A propriedade 'Instance' (com 'I' maiúsculo) é a convenção padrão.
    // O 'private set' garante que nenhum outro script possa mudar a instância.
    public static GameManager Instance { get; private set; }

    [Header("Catálogo de Fases")]
    [Tooltip("Esta lista é preenchida automaticamente da pasta 'Resources/Levels'.")]
    public List<LevelData> allLevels; // Mantido público para o MenuManager acessar

    [Header("Estado do Jogo")]
    public LevelData currentLevelData { get; private set; }
    private bool isGameOver = false;

    // --- MELHORIA 2: EVENTOS PARA DESACOPLAR A UI ---
    // O GameManager agora vai "anunciar" os eventos. A UI vai "ouvir" e reagir.
    public static event Action<float> OnManaChanged;
    public static event Action<bool> OnGameOver; // bool -> true para vitória, false para derrota

    void Awake()
    {
        // Lógica do Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("<color=green>[GameManager] Instância principal definida e marcada como DontDestroyOnLoad.</color>");
            LoadAllLevelsFromResources();
        }
        else
        {
            Debug.LogWarning($"[GameManager] Instância duplicada encontrada. Destruindo o objeto '{this.gameObject.name}'.");
            Destroy(gameObject);
            return; // Retorna para não executar o resto do código
        }
    }

    private void OnEnable()
    {
        // Registra o método para ser chamado sempre que uma nova cena for carregada
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // É uma boa prática remover o listener quando o objeto é desativado/destruído
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Método de debug que você já tinha, para confirmar que o GameManager persiste.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"<color=orange>CENA CARREGADA: '{scene.name}'.</color>");
        if (currentLevelData != null)
        {
            Debug.Log($"<color=green>Dados da fase persistiram! Iniciando fase: '{currentLevelData.levelName}'.</color>");
        }
    }

    // Sua ótima função para carregar as fases automaticamente. Mantida 100%.
    private void LoadAllLevelsFromResources()
    {
        // Carrega todos os ScriptableObjects do tipo LevelData da pasta Resources/Levels
        LevelData[] loadedLevels = Resources.LoadAll<LevelData>("Levels");
        allLevels = new List<LevelData>(loadedLevels);

        // Ordena a lista pelo 'levelIndex' para garantir que "Fase 1" venha antes da "Fase 2", etc.
        allLevels = allLevels.OrderBy(level => level.levelIndex).ToList();
        Debug.Log($"[GameManager] {allLevels.Count} fases foram carregadas da pasta Resources.");
    }

    /// <summary>
    /// O método principal para carregar uma fase. Recebe os dados do MenuManager.
    /// </summary>
    public void LoadLevel(LevelData levelToLoad)
    {
        if (levelToLoad == null)
        {
            Debug.LogError("Tentativa de carregar uma fase nula (LevelData)!");
            return;
        }

        this.currentLevelData = levelToLoad;
        isGameOver = false; // Reseta o estado de 'game over'

        Debug.Log($"<color=cyan>[GameManager] Carregando a cena '{levelToLoad.sceneToLoad}' para a fase '{levelToLoad.levelName}'</color>");
        SceneManager.LoadScene(levelToLoad.sceneToLoad);
    }

    /// <summary>
    /// Outros scripts (como o Totem) chamarão este método para atualizar a mana.
    /// O GameManager então notifica a UI através de um evento.
    /// </summary>
    public void UpdateMana(float newManaValue)
    {
        // Dispara o evento, notificando qualquer script que esteja ouvindo (como o UIManager)
        OnManaChanged?.Invoke(newManaValue);
    }

    /// <summary>
    /// Gerencia o fim do jogo (vitória ou derrota).
    /// </summary>
    public void HandleGameOver(bool playerWon)
    {
        if (isGameOver) return; // Evita que o método seja chamado múltiplas vezes
        isGameOver = true;

        // Dispara o evento de Fim de Jogo. O UIManager vai ouvir isso e mostrar o painel correto.
        OnGameOver?.Invoke(playerWon);

        if (playerWon)
        {
            Debug.Log("CONDIÇÃO DE VITÓRIA ATINGIDA!");
            StartCoroutine(EndSequence());
        }
        else
        {
            Debug.Log("CONDIÇÃO DE DERROTA ATINGIDA!");
            StartCoroutine(EndSequence());
        }
    }

    // Sequência final que pausa, espera e volta para o menu.
    private IEnumerator EndSequence()
    {
        Time.timeScale = 0f; // Pausa o jogo
        yield return new WaitForSecondsRealtime(3f); // Espera 3 segundos em tempo real
        Time.timeScale = 1f; // Despausa o jogo
        SceneManager.LoadScene("MenuScene"); // Volta para o menu
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
            // Caso de segurança: se não souber qual fase reiniciar, volta para o menu
            SceneManager.LoadScene("MenuScene");
        }
    }
}