// LevelButton.cs

using UnityEngine;
using UnityEngine.UI; // Necess�rio para o componente Button
using TMPro;          // Necess�rio para o componente TextMeshPro
using UnityEngine.SceneManagement; // Necess�rio para carregar cenas

public class LevelButton : MonoBehaviour
{
    // --- Vari�veis Configur�veis no Inspector ---

    [Header("Refer�ncias de Componentes")]
    [Tooltip("Arraste o componente Text (TMP) que mostrar� o nome da fase aqui.")]
    [SerializeField] private TMP_Text levelNameText;

    [Tooltip("Arraste o componente Button deste pr�prio objeto aqui.")]
    [SerializeField] private Button buttonComponent;

    // --- Vari�veis Privadas ---

    private LevelData _levelData;

    /// <summary>
    /// M�todo p�blico chamado pelo LevelSelectController para inicializar este bot�o.
    /// </summary>
    public void Setup(LevelData levelDataToSetup)
    {
        _levelData = levelDataToSetup;
        levelNameText.text = _levelData.name;

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(OnButtonClick);
    }

    /// <summary>
    /// Este m�todo � executado quando o jogador clica no bot�o.
    /// </summary>
    private void OnButtonClick()
    {
        // --- MODIFICA��O ADICIONADA AQUI ---
        // Esta linha � nosso "detetive". Se ela aparecer no Console, sabemos que o clique funcionou.
        Debug.Log($"<color=cyan>BOT�O CLICADO!</color> Preparando para carregar a fase '{_levelData.name}' na cena 'Game'.");

        // 1. Informa ao GameManager qual fase foi selecionada.
        GameManager.instance.SetSelectedLevel(_levelData);

        // 2. Carrega a cena principal do jogo.
        // O nome "Game" agora corresponde �s suas Build Settings.
        SceneManager.LoadScene("Game");
    }
}