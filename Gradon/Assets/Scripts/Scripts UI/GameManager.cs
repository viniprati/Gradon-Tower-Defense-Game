// GameManager.cs (Vers�o Final e Completa)

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // --- MELHORIA 1: SINGLETON PADR�O C# ---
    // A propriedade 'Instance' (com 'I' mai�sculo) � a conven��o padr�o.
    // O 'private set' garante que nenhum outro script possa mudar a inst�ncia.
    public static GameManager Instance { get; private set; }

    [Header("Cat�logo de Fases")]
    [Tooltip("Esta lista � preenchida automaticamente da pasta 'Resources/Levels'.")]
    public List<LevelData> allLevels; // Mantido p�blico para o MenuManager acessar

    [Header("Estado do Jogo")]
    public LevelData currentLevelData { get; private set; }
    private bool isGameOver = false;

    // --- MELHORIA 2: EVENTOS PARA DESACOPLAR A UI ---
    // O GameManager agora vai "anunciar" os eventos. A UI vai "ouvir" e reagir.
    public static event Action<float> OnManaChanged;
    public static event Action<bool> OnGameOver; // bool -> true para vit�ria, false para derrota

    void Awake()
    {
        // L�gica do Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("<color=green>[GameManager] Inst�ncia principal definida e marcada como DontDestroyOnLoad.</color>");
            LoadAllLevelsFromResources();
        }
        else
        {
            Debug.LogWarning($"[GameManager] Inst�ncia duplicada encontrada. Destruindo o objeto '{this.gameObject.name}'.");
            Destroy(gameObject);
            return; // Retorna para n�o executar o resto do c�digo
        }
    }

    private void OnEnable()
    {
        // Registra o m�todo para ser chamado sempre que uma nova cena for carregada
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // � uma boa pr�tica remover o listener quando o objeto � desativado/destru�do
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // M�todo de debug que voc� j� tinha, para confirmar que o GameManager persiste.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"<color=orange>CENA CARREGADA: '{scene.name}'.</color>");
        if (currentLevelData != null)
        {
            Debug.Log($"<color=green>Dados da fase persistiram! Iniciando fase: '{currentLevelData.levelName}'.</color>");
        }
    }

    // Sua �tima fun��o para carregar as fases automaticamente. Mantida 100%.
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
    /// O m�todo principal para carregar uma fase. Recebe os dados do MenuManager.
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
    /// Outros scripts (como o Totem) chamar�o este m�todo para atualizar a mana.
    /// O GameManager ent�o notifica a UI atrav�s de um evento.
    /// </summary>
    public void UpdateMana(float newManaValue)
    {
        // Dispara o evento, notificando qualquer script que esteja ouvindo (como o UIManager)
        OnManaChanged?.Invoke(newManaValue);
    }

    /// <summary>
    /// Gerencia o fim do jogo (vit�ria ou derrota).
    /// </summary>
    public void HandleGameOver(bool playerWon)
    {
        if (isGameOver) return; // Evita que o m�todo seja chamado m�ltiplas vezes
        isGameOver = true;

        // Dispara o evento de Fim de Jogo. O UIManager vai ouvir isso e mostrar o painel correto.
        OnGameOver?.Invoke(playerWon);

        if (playerWon)
        {
            Debug.Log("CONDI��O DE VIT�RIA ATINGIDA!");
            StartCoroutine(EndSequence());
        }
        else
        {
            Debug.Log("CONDI��O DE DERROTA ATINGIDA!");
            StartCoroutine(EndSequence());
        }
    }

    // Sequ�ncia final que pausa, espera e volta para o menu.
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
            // Caso de seguran�a: se n�o souber qual fase reiniciar, volta para o menu
            SceneManager.LoadScene("MenuScene");
        }
    }
}