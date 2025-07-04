// BuildManager.cs
using UnityEngine;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    // Padrão Singleton: permite que qualquer script acesse este manager facilmente.
    public static BuildManager instance;

    private PlayerController playerController;
    private List<GameObject> availableTowers;
    private int selectedTowerIndex = 0;
    private GameObject ghostTowerInstance;
    private LayerMask towerSlotLayer;

    void Awake()
    {
        // Configuração do Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Pega as referências do PlayerController para poder usar suas variáveis
        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            this.availableTowers = playerController.availableTowers;
            this.towerSlotLayer = playerController.towerSlotLayer;
        }
    }

    void Update()
    {
        // Se estivermos no modo de construção, atualiza a posição do fantasma.
        if (ghostTowerInstance != null)
        {
            UpdateGhostTowerPosition();
        }
    }

    // Chamado pelo PlayerController para entrar no modo de construção
    public void EnterBuildMode()
    {
        if (availableTowers.Count == 0) return;
        selectedTowerIndex = 0;
        SpawnGhostTower();
    }

    // Chamado pelo PlayerController para sair/cancelar
    public void ExitBuildMode()
    {
        if (ghostTowerInstance != null)
        {
            Destroy(ghostTowerInstance);
        }
    }

    // Chamado pelo PlayerController para trocar a torre
    public void SelectNextTower()
    {
        selectedTowerIndex++;
        if (selectedTowerIndex >= availableTowers.Count) selectedTowerIndex = 0;
        UpdateGhostTower();
    }

    public void SelectPreviousTower()
    {
        selectedTowerIndex--;
        if (selectedTowerIndex < 0) selectedTowerIndex = availableTowers.Count - 1;
        UpdateGhostTower();
    }

    // O coração da lógica de construção
    public void TryPlaceTower(Vector3 playerPosition)
    {
        if (ghostTowerInstance == null || !ghostTowerInstance.activeSelf)
        {
            Debug.Log("Local inválido para construção!");
            return;
        }

        GameObject towerToBuildPrefab = availableTowers[selectedTowerIndex];

        // Assumindo que a torre tem um script 'SamuraiT' ou similar com um custo
        // Vamos pegar o custo do prefab.
        int towerCost = 0; // Você precisa implementar a lógica de custo nas suas torres
        // Exemplo: if (towerToBuildPrefab.GetComponent<SamuraiT>() != null) towerCost = towerToBuildPrefab.GetComponent<SamuraiT>().cost;

        if (playerController.currentMana >= towerCost)
        {
            // TEM MANA SUFICIENTE
            playerController.SpendMana(towerCost);

            // Encontra o slot e constrói a torre REAL
            Collider2D slotCollider = Physics2D.OverlapCircle(playerPosition, 0.2f, towerSlotLayer);
            if (slotCollider != null)
            {
                // AQUI ESTÁ A LINHA QUE CRIA A TORRE DE VERDADE
                Instantiate(towerToBuildPrefab, slotCollider.transform.position, Quaternion.identity);
                Debug.Log(towerToBuildPrefab.name + " construída com sucesso!");
                // Adicionar lógica para marcar o slot como ocupado aqui.
            }
        }
        else
        {
            // NÃO TEM MANA
            Debug.Log("Mana insuficiente para construir " + towerToBuildPrefab.name);
        }
    }

    // --- Funções Auxiliares para a pré-visualização ---

    private void UpdateGhostTower()
    {
        if (ghostTowerInstance != null) Destroy(ghostTowerInstance);
        SpawnGhostTower();
    }

    private void SpawnGhostTower()
    {
        ghostTowerInstance = Instantiate(availableTowers[selectedTowerIndex]);

        ghostTowerInstance.GetComponent<Collider2D>().enabled = false;
        if (ghostTowerInstance.GetComponent<SamuraiT>() != null) ghostTowerInstance.GetComponent<SamuraiT>().enabled = false;

        SpriteRenderer sr = ghostTowerInstance.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1f, 1f, 1f, 0.5f);
    }

    private void UpdateGhostTowerPosition()
    {
        Collider2D slotCollider = Physics2D.OverlapCircle(playerController.transform.position, 0.2f, towerSlotLayer);

        if (slotCollider != null)
        {
            ghostTowerInstance.transform.position = slotCollider.transform.position;
            ghostTowerInstance.SetActive(true);
        }
        else
        {
            ghostTowerInstance.SetActive(false);
        }
    }
}