// LevelButton.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [Header("Refer�ncias de Componentes")]
    [Tooltip("Arraste o componente Text (TMP) que mostrar� o nome da fase aqui.")]
    [SerializeField] private TMP_Text levelNameText;

    [Tooltip("Arraste o componente Button deste pr�prio objeto aqui.")]
    [SerializeField] private Button buttonComponent;

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
        // 1. Informa ao GameManager qual fase foi selecionada.
        GameManager.instance.SetSelectedLevel(_levelData);

        // 2. Carrega a cena principal do jogo.
        SceneManager.LoadScene("Game");
    }
}