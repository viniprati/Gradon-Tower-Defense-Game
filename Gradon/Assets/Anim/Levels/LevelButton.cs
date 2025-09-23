// LevelButton.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TMP_Text levelNameText;
    [SerializeField] private Button buttonComponent;

    private LevelData _levelData;

    public void Setup(LevelData levelDataToSetup)
    {
        _levelData = levelDataToSetup;
        levelNameText.text = _levelData.name;

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        // CORREÇÃO AQUI: de Instance para instance
        GameManager.instance.SetSelectedLevel(_levelData);

        // Certifique-se de que o nome da cena está correto
        SceneManager.LoadScene("GameScene");
    }
}