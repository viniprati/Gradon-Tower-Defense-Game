// MenuManager.cs

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Essencial para usar TextMeshPro
using System.Collections.Generic; // Essencial para usar Listas

public class MenuManager : MonoBehaviour
{
    // --- NOVAS VARI�VEIS PARA A SELE��O DE FASES ---
    [Header("Refer�ncias da UI de Sele��o")]
    [Tooltip("Arraste aqui o objeto de texto que exibir� o nome e o n�mero da fase.")]
    [SerializeField] private TextMeshProUGUI levelInfoText;

    // Vari�veis internas
    private int selectedLevelIndex = 0; // Guarda o �ndice da fase selecionada na lista (0, 1, 2...)
    private List<LevelData> allLevels; // Uma c�pia da lista de fases do GameManager

    /// <summary>
    /// Chamado pela Unity quando o script � carregado.
    /// </summary>
    void Start()
    {
        // Pega a lista de todas as fases dispon�veis no GameManager
        if (GameManager.instance != null)
        {
            allLevels = GameManager.instance.allLevels;
        }
        else
        {
            Debug.LogError("GameManager n�o foi encontrado! A sele��o de fases n�o funcionar�.");
            if (levelInfoText != null) levelInfoText.text = "ERRO";
            return;
        }

        // Garante que temos pelo menos uma fase para mostrar
        if (allLevels == null || allLevels.Count == 0)
        {
            if (levelInfoText != null) levelInfoText.text = "Nenhuma Fase Dispon�vel";
            return;
        }

        // Exibe a primeira fase da lista
        UpdateLevelDisplay();
    }

    // --- NOVOS M�TODOS PARA OS BOT�ES DE SETA ---

    /// <summary>
    /// Esta fun��o ser� chamada pelo seu bot�o da seta para a DIREITA.
    /// </summary>
    public void NextLevel()
    {
        if (allLevels.Count == 0) return;

        selectedLevelIndex++;
        // Se chegarmos ao final da lista, damos a volta e voltamos para o come�o.
        if (selectedLevelIndex >= allLevels.Count)
        {
            selectedLevelIndex = 0;
        }
        UpdateLevelDisplay();
    }

    /// <summary>
    /// Esta fun��o ser� chamada pelo seu bot�o da seta para a ESQUERDA.
    /// </summary>
    public void PreviousLevel()
    {
        if (allLevels.Count == 0) return;

        selectedLevelIndex--;
        // Se voltarmos do come�o, vamos para o final da lista.
        if (selectedLevelIndex < 0)
        {
            selectedLevelIndex = allLevels.Count - 1;
        }
        UpdateLevelDisplay();
    }

    /// <summary>
    /// Atualiza o texto na tela com as informa��es da fase atualmente selecionada.
    /// </summary>
    private void UpdateLevelDisplay()
    {
        // Pega os dados da fase selecionada na lista
        LevelData selectedLevelData = allLevels[selectedLevelIndex];

        // Atualiza o texto da UI
        if (levelInfoText != null)
        {
            levelInfoText.text = $"{selectedLevelData.levelIndex}. {selectedLevelData.levelName}";
        }
    }

    // --- FUN��O ATUALIZADA PARA O BOT�O "INICIAR" ---
    /// <summary>
    /// Esta fun��o ser� chamada pelo seu bot�o "Iniciar".
    /// Ela pega a fase selecionada e manda o GameManager carreg�-la.
    /// </summary>
    public void StartGame()
    {
        if (allLevels.Count > 0 && GameManager.instance != null)
        {
            // Pega o N�MERO real da fase (o levelIndex que voc� configurou no asset)
            int levelToLoad = allLevels[selectedLevelIndex].levelIndex;

            // Pede ao GameManager para carregar essa fase
            GameManager.instance.LoadLevel(levelToLoad);
        }
        else
        {
            Debug.LogError("N�o foi poss�vel iniciar a fase. Verifique a configura��o do GameManager e se h� fases na lista.", this.gameObject);
        }
    }

    /// <summary>
    /// Fun��o para fechar o jogo.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}