// LevelSelectControler
using UnityEngine;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform buttonContainer;

    void Start()
    {
        PopulateLevelButtons();
    }

    private void PopulateLevelButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // CORRE��O AQUI: de Instance para instance
        if (GameManager.instance == null || GameManager.instance.allLevels == null)
        {
            Debug.LogError("GameManager ou a lista 'allLevels' n�o foi encontrada!");
            return;
        }

        // CORRE��O AQUI: de Instance para instance (e de levelCatalog para allLevels)
        foreach (LevelData levelData in GameManager.instance.allLevels)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, buttonContainer);
            LevelButton levelButton = buttonGO.GetComponent<LevelButton>();

            if (levelButton != null)
            {
                levelButton.Setup(levelData);
            }
            else
            {
                Debug.LogError("O prefab do bot�o n�o tem o script LevelButton!", buttonGO);
            }
        }
    }
}