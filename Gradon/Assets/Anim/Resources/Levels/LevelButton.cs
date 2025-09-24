// LevelButton.cs

using UnityEngine;
using UnityEngine.UI; // Necessário para o componente Button
using TMPro;          // Necessário para o componente TextMeshPro
using UnityEngine.SceneManagement; // Necessário para carregar cenas

public class LevelButton : MonoBehaviour
{
    // --- Variáveis Configuráveis no Inspector ---

    [Header("Referências de Componentes")]
    [Tooltip("Arraste o componente Text (TMP) que mostrará o nome da fase aqui.")]
    [SerializeField] private TMP_Text levelNameText;

    [Tooltip("Arraste o componente Button deste próprio objeto aqui.")]
    [SerializeField] private Button buttonComponent;

    // --- Variáveis Privadas ---

    private LevelData _levelData;

    /// <summary>
    /// Método público chamado pelo LevelSelectController para inicializar este botão.
    /// </summary>
    public void Setup(LevelData levelDataToSetup)
    {
        _levelData = levelDataToSetup;
        levelNameText.text = _levelData.name;

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(OnButtonClick);
    }

    /// <summary>
    /// Este método é executado quando o jogador clica no botão.
    /// </summary>
    private void OnButtonClick()
    {
        // --- MODIFICAÇÃO ADICIONADA AQUI ---
        // Esta linha é nosso "detetive". Se ela aparecer no Console, sabemos que o clique funcionou.
        Debug.Log($"<color=cyan>BOTÃO CLICADO!</color> Preparando para carregar a fase '{_levelData.name}' na cena 'Game'.");

        // 1. Informa ao GameManager qual fase foi selecionada.
        GameManager.instance.SetSelectedLevel(_levelData);

        // 2. Carrega a cena principal do jogo.
        // O nome "Game" agora corresponde às suas Build Settings.
        SceneManager.LoadScene("Game");
    }
}