using UnityEngine;
using UnityEngine.UI;
using TMPro; // Não se esqueça de importar o TextMeshPro
using UnityEngine.SceneManagement; // Importe para carregar cenas

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TMP_Text levelNameText;
    [SerializeField] private Button buttonComponent;

    private LevelData _levelData;

    // Este é o método chave que o Controller vai chamar
    public void Setup(LevelData levelDataToSetup)
    {
        _levelData = levelDataToSetup;
        levelNameText.text = _levelData.name; // Usa o nome do Scriptable Object como nome do botão

        // Remove qualquer listener antigo e adiciona o novo. Boa prática.
        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        // 1. Avisa o GameManager qual fase foi escolhida
        GameManager.Instance.SetSelectedLevel(_levelData);

        // 2. Carrega a cena principal do jogo
        // Certifique-se de que o nome "GameScene" corresponde ao nome da sua cena de gameplay
        SceneManager.LoadScene("GameScene");
    }
}