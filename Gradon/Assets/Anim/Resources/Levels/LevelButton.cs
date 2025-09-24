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

    // Guarda a refer�ncia para os dados da fase que este bot�o representa.
    private LevelData _levelData;

    /// <summary>
    /// M�todo p�blico chamado pelo LevelSelectController para inicializar este bot�o.
    /// </summary>
    /// <param name="levelDataToSetup">Os dados da fase que este bot�o ir� carregar.</param>
    public void Setup(LevelData levelDataToSetup)
    {
        // 1. Armazena os dados da fase recebidos.
        _levelData = levelDataToSetup;

        // 2. Atualiza o texto do bot�o para mostrar o nome da fase.
        //    O ".name" vem diretamente do nome do seu arquivo Scriptable Object (ex: "Level1.asset").
        levelNameText.text = _levelData.name;

        // 3. Configura o evento de clique do bot�o.
        //    Primeiro, removemos qualquer listener antigo para evitar cliques duplos.
        buttonComponent.onClick.RemoveAllListeners();
        //    Depois, adicionamos nosso m�todo OnButtonClick para ser chamado quando o bot�o for clicado.
        buttonComponent.onClick.AddListener(OnButtonClick);
    }

    /// <summary>
    /// Este m�todo � executado quando o jogador clica no bot�o.
    /// </summary>
    private void OnButtonClick()
    {
        // 1. Informa ao GameManager (que persiste entre as cenas) qual fase foi selecionada.
        //    Isso � crucial para que a pr�xima cena saiba quais dados de fase carregar.
        GameManager.instance.SetSelectedLevel(_levelData);

        // 2. Carrega a cena principal do jogo.
        //    IMPORTANTE: Certifique-se de que o nome da sua cena de gameplay � exatamente "GameScene".
        //    Verifique em File -> Build Settings se a cena est� adicionada.
        SceneManager.LoadScene("Game");
    }
}