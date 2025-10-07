// MenuManager.cs

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Essencial para usar TextMeshPro
using System.Collections.Generic; // Essencial para usar Listas

public class MenuManager : MonoBehaviour
{
    // --- NOVAS VARIÁVEIS PARA A SELEÇÃO DE FASES ---
    [Header("Referências da UI de Seleção")]
    [Tooltip("Arraste aqui o objeto de texto que exibirá o nome e o número da fase.")]
    [SerializeField] private TextMeshProUGUI levelInfoText;

    // Variáveis internas
    private int selectedLevelIndex = 0; // Guarda o índice da fase selecionada na lista (0, 1, 2...)
    private List<LevelData> allLevels; // Uma cópia da lista de fases do GameManager

    /// <summary>
    /// Chamado pela Unity quando o script é carregado.
    /// </summary>
    void Start()
    {
        // Pega a lista de todas as fases disponíveis no GameManager
        if (GameManager.instance != null)
        {
            allLevels = GameManager.instance.allLevels;
        }
        else
        {
            Debug.LogError("GameManager não foi encontrado! A seleção de fases não funcionará.");
            if (levelInfoText != null) levelInfoText.text = "ERRO";
            return;
        }

        // Garante que temos pelo menos uma fase para mostrar
        if (allLevels == null || allLevels.Count == 0)
        {
            if (levelInfoText != null) levelInfoText.text = "Nenhuma Fase Disponível";
            return;
        }

        // Exibe a primeira fase da lista
        UpdateLevelDisplay();
    }

    // --- NOVOS MÉTODOS PARA OS BOTÕES DE SETA ---

    /// <summary>
    /// Esta função será chamada pelo seu botão da seta para a DIREITA.
    /// </summary>
    public void NextLevel()
    {
        if (allLevels.Count == 0) return;

        selectedLevelIndex++;
        // Se chegarmos ao final da lista, damos a volta e voltamos para o começo.
        if (selectedLevelIndex >= allLevels.Count)
        {
            selectedLevelIndex = 0;
        }
        UpdateLevelDisplay();
    }

    /// <summary>
    /// Esta função será chamada pelo seu botão da seta para a ESQUERDA.
    /// </summary>
    public void PreviousLevel()
    {
        if (allLevels.Count == 0) return;

        selectedLevelIndex--;
        // Se voltarmos do começo, vamos para o final da lista.
        if (selectedLevelIndex < 0)
        {
            selectedLevelIndex = allLevels.Count - 1;
        }
        UpdateLevelDisplay();
    }

    /// <summary>
    /// Atualiza o texto na tela com as informações da fase atualmente selecionada.
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

    // --- FUNÇÃO ATUALIZADA PARA O BOTÃO "INICIAR" ---
    /// <summary>
    /// Esta função será chamada pelo seu botão "Iniciar".
    /// Ela pega a fase selecionada e manda o GameManager carregá-la.
    /// </summary>
    public void StartGame()
    {
        if (allLevels.Count > 0 && GameManager.instance != null)
        {
            // Pega o NÚMERO real da fase (o levelIndex que você configurou no asset)
            int levelToLoad = allLevels[selectedLevelIndex].levelIndex;

            // Pede ao GameManager para carregar essa fase
            GameManager.instance.LoadLevel(levelToLoad);
        }
        else
        {
            Debug.LogError("Não foi possível iniciar a fase. Verifique a configuração do GameManager e se há fases na lista.", this.gameObject);
        }
    }

    /// <summary>
    /// Função para fechar o jogo.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}