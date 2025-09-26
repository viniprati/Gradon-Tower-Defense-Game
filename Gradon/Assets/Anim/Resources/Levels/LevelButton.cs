// LevelButton.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [Header("Referências de Componentes")]
    [Tooltip("Arraste o componente Text (TMP) que mostrará o nome da fase aqui.")]
    [SerializeField] private TMP_Text levelNameText;

    [Tooltip("Arraste o componente Button deste próprio objeto aqui.")]
    [SerializeField] private Button buttonComponent;

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
        // 1. Informa ao GameManager qual fase foi selecionada.
        GameManager.instance.SetSelectedLevel(_levelData);

        // 2. Carrega a cena principal do jogo.
        SceneManager.LoadScene("Game");
    }
}