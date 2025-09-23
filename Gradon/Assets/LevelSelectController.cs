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
        // Limpa bot�es antigos, caso existam
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Verifica se o GameManager e o cat�logo existem
        if (GameManager.Instance == null || GameManager.Instance.levelCatalog == null)
        {
            Debug.LogError("GameManager ou LevelCatalog n�o encontrado!");
            return;
        }

        // Cria um bot�o para cada LevelData no cat�logo
        foreach (LevelData levelData in GameManager.Instance.levelCatalog)
        {
            // 1. Instancia o prefab do bot�o como filho do container
            GameObject buttonGO = Instantiate(levelButtonPrefab, buttonContainer);

            // 2. Pega o script LevelButton do objeto rec�m-criado
            LevelButton levelButton = buttonGO.GetComponent<LevelButton>();

            // 3. Chama o m�todo Setup para passar os dados da fase para o bot�o
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