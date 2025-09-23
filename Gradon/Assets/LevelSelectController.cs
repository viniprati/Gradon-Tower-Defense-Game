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
        // Limpa botões antigos, caso existam
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Verifica se o GameManager e o catálogo existem
        if (GameManager.Instance == null || GameManager.Instance.levelCatalog == null)
        {
            Debug.LogError("GameManager ou LevelCatalog não encontrado!");
            return;
        }

        // Cria um botão para cada LevelData no catálogo
        foreach (LevelData levelData in GameManager.Instance.levelCatalog)
        {
            // 1. Instancia o prefab do botão como filho do container
            GameObject buttonGO = Instantiate(levelButtonPrefab, buttonContainer);

            // 2. Pega o script LevelButton do objeto recém-criado
            LevelButton levelButton = buttonGO.GetComponent<LevelButton>();

            // 3. Chama o método Setup para passar os dados da fase para o botão
            if (levelButton != null)
            {
                levelButton.Setup(levelData);
            }
            else
            {
                Debug.LogError("O prefab do botão não tem o script LevelButton!", buttonGO);
            }
        }
    }
}