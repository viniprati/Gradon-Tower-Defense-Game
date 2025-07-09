// BuildManager.cs
using UnityEngine;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    // --- Padrão Singleton ---
    public static BuildManager instance;

    // --- Referências e Estado ---
    private PlayerController playerController;
    private List<GameObject> availableTowers;
    private int selectedTowerIndex = 0;
    private GameObject ghostTowerInstance;
    private bool isBuilding = false;

    // Propriedade pública para que outros scripts possam saber se estamos no modo de construção
    public bool IsInBuildMode { get { return isBuilding; } }

    void Awake()
    {
        // Configuração do Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Garante que só exista um
        }
    }

    void Start()
    {
        // Pega as referências do PlayerController para poder usar suas variáveis
        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            // Pega a lista de torres que o jogador pode construir
            this.availableTowers = playerController.availableTowers;
        }
    }

    void Update()
    {
        // Se estivermos no modo de construção, a pré-visualização segue o jogador.
        if (isBuilding && ghostTowerInstance != null)
        {
            UpdateGhostTowerPosition();
        }
    }

    // --- Métodos Públicos chamados pelo PlayerController ---

    public void ToggleBuildMode()
    {
        if (!isBuilding)
        {
            EnterBuildMode();
        }
        else
        {
            TryPlaceTower();
        }
    }

    public void SelectNextTower()
    {
        if (!isBuilding) return;
        selectedTowerIndex++;
        if (selectedTowerIndex >= availableTowers.Count) selectedTowerIndex = 0;
        UpdateGhostTower();
    }

    public void SelectPreviousTower()
    {
        if (!isBuilding) return;
        selectedTowerIndex--;
        if (selectedTowerIndex < 0) selectedTowerIndex = availableTowers.Count - 1;
        UpdateGhostTower();
    }

    // --- Lógica Interna de Construção ---

    private void EnterBuildMode()
    {
        // Verifica se há torres para construir
        if (availableTowers == null || availableTowers.Count == 0)
        {
            Debug.LogError("A lista 'Available Towers' no PlayerController está vazia!");
            return;
        }

        // Verifica se há mana suficiente para a torre mais barata (exemplo)
        int towerCost = 10; // Custo de exemplo
        if (playerController.currentMana < towerCost)
        {
            Debug.Log("Mana insuficiente para iniciar a construção!");
            return;
        }

        isBuilding = true;
        selectedTowerIndex = 0;
        SpawnGhostTower();
    }

    private void ExitBuildMode()
    {
        isBuilding = false;
        if (ghostTowerInstance != null)
        {
            Destroy(ghostTowerInstance);
        }
    }

    private void TryPlaceTower()
    {
        if (ghostTowerInstance == null) return;

        GameObject towerToBuildPrefab = availableTowers[selectedTowerIndex];
        int towerCost = 10; // Custo de exemplo

        if (playerController.currentMana >= towerCost)
        {
            playerController.SpendMana(towerCost);
            // Constrói a torre na posição do jogador
            Instantiate(towerToBuildPrefab, playerController.transform.position, Quaternion.identity);
            ExitBuildMode();
        }
        else
        {
            Debug.Log("Mana insuficiente!");
        }
    }

    // --- Funções Auxiliares da Pré-Visualização ---

    private void UpdateGhostTower()
    {
        if (ghostTowerInstance != null) Destroy(ghostTowerInstance);
        SpawnGhostTower();
    }

    private void SpawnGhostTower()
    {
        ghostTowerInstance = Instantiate(availableTowers[selectedTowerIndex]);
        ghostTowerInstance.name = "GHOST_TOWER";

        var collider = ghostTowerInstance.GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        var samuraiScript = ghostTowerInstance.GetComponent<SamuraiT>();
        if (samuraiScript != null) samuraiScript.enabled = false;

        var spriteRenderer = ghostTowerInstance.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
    }

    private void UpdateGhostTowerPosition()
    {
        if (playerController != null)
        {
            ghostTowerInstance.transform.position = playerController.transform.position;
        }
    }
}