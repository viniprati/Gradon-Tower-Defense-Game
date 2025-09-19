// LevelSelectController.cs
using UnityEngine;

public class LevelSelectController : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public Transform buttonContainer;

    void Start()
    {
        if (GameManager.instance == null) return;
        CreateLevelButtons();
    }

    private void CreateLevelButtons()
    {
        foreach (LevelData level in GameManager.instance.allLevels)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, buttonContainer);
            buttonGO.GetComponent<LevelButton>().Setup(level);
        }
    }
}