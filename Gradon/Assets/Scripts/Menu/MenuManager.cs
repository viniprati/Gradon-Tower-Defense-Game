// MenuManager.cs

using UnityEngine;
// Adicionamos esta linha para poder controlar as cenas.
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // --- VARIÁVEIS REMOVIDAS ---
    // Não precisamos mais das referências dos painéis, pois não vamos mais trocá-los.
    // [SerializeField] private GameObject mainMenuPanel;
    // [SerializeField] private GameObject levelSelectPanel;

    // --- NOVA VARIÁVEL ADICIONADA ---
    [Header("Configuração de Jogo Rápido")]
    [Tooltip("Arraste o arquivo LevelData da fase que deve ser carregada ao clicar em 'Iniciar'. (Ex: Level 1)")]
    [SerializeField] private LevelData levelToStart;

    // O método Start não é mais necessário para esta lógica.
    // void Start() { ... }

    /// <summary>
    /// Esta é a NOVA função que o botão "Iniciar" vai chamar.
    /// Ela prepara o GameManager e carrega a cena do jogo diretamente.
    /// </summary>
    public void StartGame()
    {
        // Verificação de Segurança #1: Garante que um LevelData foi arrastado no Inspector.
        if (levelToStart == null)
        {
            Debug.LogError("Nenhum 'LevelData' foi definido no campo 'Level To Start' do MenuManager! Não sei qual fase carregar.", this.gameObject);
            return; // Para a execução para evitar erros.
        }

        // Verificação de Segurança #2: Garante que o GameManager está pronto.
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager não foi encontrado! Não é possível iniciar o jogo. Verifique se o objeto GameManager está na MenuScene.", this.gameObject);
            return;
        }

        // 1. Avisa ao GameManager qual fase deve ser carregada.
        GameManager.instance.SetSelectedLevel(levelToStart);

        // 2. Carrega a cena do jogo.
        SceneManager.LoadScene("Game");
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