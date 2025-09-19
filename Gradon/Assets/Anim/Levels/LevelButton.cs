// LevelButton.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI levelNameText;
    public Button buttonComponent;
    private int levelIndex;

    // Este método é chamado pelo 'construtor' para configurar o botão
    public void Setup(LevelData levelData)
    {
        this.levelIndex = levelData.levelIndex;
        levelNameText.text = levelData.levelName;

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(LoadLevel);
    }

    private void LoadLevel()
    {
        GameManager.instance.LoadLevel(levelIndex);
    }
}