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

    // Guarda a referência para os dados da fase que este botão representa.
    private LevelData _levelData;

    /// <summary>
    /// Método público chamado pelo LevelSelectController para inicializar este botão.
    /// </summary>
    /// <param name="levelDataToSetup">Os dados da fase que este botão irá carregar.</param>
    public void Setup(LevelData levelDataToSetup)
    {
        // 1. Armazena os dados da fase recebidos.
        _levelData = levelDataToSetup;

        // 2. Atualiza o texto do botão para mostrar o nome da fase.
        //    O ".name" vem diretamente do nome do seu arquivo Scriptable Object (ex: "Level1.asset").
        levelNameText.text = _levelData.name;

        // 3. Configura o evento de clique do botão.
        //    Primeiro, removemos qualquer listener antigo para evitar cliques duplos.
        buttonComponent.onClick.RemoveAllListeners();
        //    Depois, adicionamos nosso método OnButtonClick para ser chamado quando o botão for clicado.
        buttonComponent.onClick.AddListener(OnButtonClick);
    }

    /// <summary>
    /// Este método é executado quando o jogador clica no botão.
    /// </summary>
    private void OnButtonClick()
    {
        // 1. Informa ao GameManager (que persiste entre as cenas) qual fase foi selecionada.
        //    Isso é crucial para que a próxima cena saiba quais dados de fase carregar.
        GameManager.instance.SetSelectedLevel(_levelData);

        // 2. Carrega a cena principal do jogo.
        //    IMPORTANTE: Certifique-se de que o nome da sua cena de gameplay é exatamente "GameScene".
        //    Verifique em File -> Build Settings se a cena está adicionada.
        SceneManager.LoadScene("Game");
    }
}