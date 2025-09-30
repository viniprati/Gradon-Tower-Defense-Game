// MenuManager.cs

using UnityEngine;
// Adicionamos esta linha para poder controlar as cenas.
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // --- VARI�VEIS REMOVIDAS ---
    // N�o precisamos mais das refer�ncias dos pain�is, pois n�o vamos mais troc�-los.
    // [SerializeField] private GameObject mainMenuPanel;
    // [SerializeField] private GameObject levelSelectPanel;

    // --- NOVA VARI�VEL ADICIONADA ---
    [Header("Configura��o de Jogo R�pido")]
    [Tooltip("Arraste o arquivo LevelData da fase que deve ser carregada ao clicar em 'Iniciar'. (Ex: Level 1)")]
    [SerializeField] private LevelData levelToStart;

    // O m�todo Start n�o � mais necess�rio para esta l�gica.
    // void Start() { ... }

    /// <summary>
    /// Esta � a NOVA fun��o que o bot�o "Iniciar" vai chamar.
    /// Ela prepara o GameManager e carrega a cena do jogo diretamente.
    /// </summary>
    public void StartGame()
    {
        // Verifica��o de Seguran�a #1: Garante que um LevelData foi arrastado no Inspector.
        if (levelToStart == null)
        {
            Debug.LogError("Nenhum 'LevelData' foi definido no campo 'Level To Start' do MenuManager! N�o sei qual fase carregar.", this.gameObject);
            return; // Para a execu��o para evitar erros.
        }

        // Verifica��o de Seguran�a #2: Garante que o GameManager est� pronto.
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager n�o foi encontrado! N�o � poss�vel iniciar o jogo. Verifique se o objeto GameManager est� na MenuScene.", this.gameObject);
            return;
        }

        // 1. Avisa ao GameManager qual fase deve ser carregada.
        GameManager.instance.SetSelectedLevel(levelToStart);

        // 2. Carrega a cena do jogo.
        SceneManager.LoadScene("Game");
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