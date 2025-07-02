using UnityEngine;
// Se for usar UI, adicione esta linha
// using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance; // Singleton

    [Header("Configuração das Torres")]
    public GameObject[] towerPrefabs; // Array para os 3 prefabs das torres
    public int[] towerCosts; // Array para os custos de cada torre (na mesma ordem dos prefabs)

    [Header("Estado do Jogo")]
    public int currentGold = 100; // Ouro inicial

    private int selectedTowerIndex = 0;
    private bool isInBuildMode = false;

    // Referência para a UI (opcional por enquanto)
    // public GameObject buildUI;
    // public Text selectedTowerText;


    void Awake()
    {
        // Configuração do Singleton
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void EnterBuildMode()
    {
        isInBuildMode = true;
        Debug.Log("Modo de construção ATIVADO. Torre selecionada: " + towerPrefabs[selectedTowerIndex].name);
        // Aqui você ativaria a sua UI
        // buildUI.SetActive(true);
        // UpdateUI();
    }

    public void ExitBuildMode()
    {
        isInBuildMode = false;
        Debug.Log("Modo de construção DESATIVADO.");
        // Aqui você desativaria a sua UI
        // buildUI.SetActive(false);
    }

    public void SelectNextTower()
    {
        if (!isInBuildMode) return;

        selectedTowerIndex++;
        if (selectedTowerIndex >= towerPrefabs.Length)
        {
            selectedTowerIndex = 0; // Volta para a primeira
        }
        Debug.Log("Torre selecionada: " + towerPrefabs[selectedTowerIndex].name);
        // UpdateUI();
    }

    public void SelectPreviousTower()
    {
        if (!isInBuildMode) return;

        selectedTowerIndex--;
        if (selectedTowerIndex < 0)
        {
            selectedTowerIndex = towerPrefabs.Length - 1; // Vai para a última
        }
        Debug.Log("Torre selecionada: " + towerPrefabs[selectedTowerIndex].name);
        // UpdateUI();
    }

    public void TryPlaceTower(Vector3 position)
    {
        int cost = towerCosts[selectedTowerIndex];

        if (currentGold >= cost)
        {
            // Tem ouro suficiente
            currentGold -= cost;
            Instantiate(towerPrefabs[selectedTowerIndex], position, Quaternion.identity);
            Debug.Log("Torre construída! Ouro restante: " + currentGold);
        }
        else
        {
            // Não tem ouro
            Debug.Log("Ouro insuficiente! Precisa de " + cost);
        }
    }

    // Função para atualizar a UI (vamos deixar para depois, mas a lógica ficaria aqui)
    // void UpdateUI() {
    //     selectedTowerText.text = $"Torre: {towerPrefabs[selectedTowerIndex].name}\nCusto: {towerCosts[selectedTowerIndex]}";
    // }
}