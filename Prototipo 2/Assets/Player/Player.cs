// PlayerController.cs
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float moveSpeed = 5f;

    [Header("Recursos do Jogador")]
    public float maxMana = 100f;
    public float currentMana;

    [Header("Construção de Torres")]
    public List<GameObject> availableTowers;
    public LayerMask towerSlotLayer;

    // --- Variáveis Internas ---
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isBuilding = false;
    private bool isFacingRight = true;
    private int selectedTowerIndex = 0;
    private GameObject ghostTowerInstance;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentMana = maxMana;
    }

    void Update()
    {
        if (isBuilding)
        {
            HandleBuildMode();
        }
        else
        {
            HandleMovementMode();
        }
    }

    void FixedUpdate()
    {
        // CORREÇÃO: A propriedade correta é 'velocity'
        if (!isBuilding)
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // --- LÓGICA DE CADA MODO ---

    private void HandleMovementMode()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();
        HandleFlip();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            EnterBuildMode();
        }
    }

    // --- MÉTODO HandleBuildMode MODIFICADO ---
    private void HandleBuildMode()
    {
        // Selecionar torre com A e D
        if (Input.GetKeyDown(KeyCode.A)) CycleTowerSelection(-1);
        if (Input.GetKeyDown(KeyCode.D)) CycleTowerSelection(1);

        // NOVO: Cancelar com a tecla S
        if (Input.GetKeyDown(KeyCode.S))
        {
            ExitBuildMode();
            return; // Sai da função para não tentar construir no mesmo frame
        }

        // Construir com ESPAÇO
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // NOVA LÓGICA: Chama a função que tenta construir a torre
            TryPlaceTower();
        }

        UpdateGhostTowerPosition();
    }

    // --- FUNÇÃO TryPlaceTower (NOVA) ---
    private void TryPlaceTower()
    {
        // Verifica se a pré-visualização está ativa (ou seja, se o local é válido)
        if (ghostTowerInstance == null || !ghostTowerInstance.activeSelf)
        {
            Debug.Log("Não é possível construir aqui! Encontre um local válido.");
            return;
        }

        // Pega o prefab da torre selecionada
        GameObject towerToBuildPrefab = availableTowers[selectedTowerIndex];

        // --- LÓGICA DE CUSTO ---
        // Pega o custo do script da torre (assumindo que todas têm um script de torre com custo)
        int towerCost = 0;
        var samuraiScript = towerToBuildPrefab.GetComponent<SamuraiT>();
        // Adicione 'else if' para outras torres (Dragon, Kirin, etc.)
        if (samuraiScript != null)
        {
            // Aqui você precisaria adicionar uma variável 'public int cost' no seu script SamuraiT
            // towerCost = samuraiScript.cost; 
        }

        // Verifica se tem mana suficiente
        if (currentMana >= towerCost)
        {
            // Gasta a mana
            SpendMana(towerCost);

            // Pega a posição do fantasma para construir a torre real
            Vector3 buildPosition = ghostTowerInstance.transform.position;

            // CRIA A TORRE DE VERDADE
            Instantiate(towerToBuildPrefab, buildPosition, Quaternion.identity);
            Debug.Log(towerToBuildPrefab.name + " construída com sucesso!");

            // Adicionar lógica para marcar o slot como ocupado aqui (se necessário)

            // Sai do modo de construção após o sucesso
            ExitBuildMode();
        }
        else
        {
            Debug.Log("Mana insuficiente para construir " + towerToBuildPrefab.name + "!");
        }
    }


    // --- FUNÇÕES DO MODO DE CONSTRUÇÃO (sem grandes alterações) ---
    private void EnterBuildMode()
    {
        isBuilding = true;
        Debug.Log("Entrou no modo de construção.");
        selectedTowerIndex = 0;
        SpawnGhostTower();
    }

    private void ExitBuildMode()
    {
        isBuilding = false;
        Debug.Log("Saiu do modo de construção.");
        if (ghostTowerInstance != null)
        {
            Destroy(ghostTowerInstance);
        }
    }

    private void CycleTowerSelection(int direction)
    {
        selectedTowerIndex += direction;
        if (selectedTowerIndex >= availableTowers.Count) selectedTowerIndex = 0;
        if (selectedTowerIndex < 0) selectedTowerIndex = availableTowers.Count - 1;

        Destroy(ghostTowerInstance);
        SpawnGhostTower();
        Debug.Log("Torre selecionada: " + availableTowers[selectedTowerIndex].name);
    }

    private void SpawnGhostTower()
    {
        if (availableTowers.Count == 0) return;
        ghostTowerInstance = Instantiate(availableTowers[selectedTowerIndex]);

        ghostTowerInstance.GetComponent<Collider2D>().enabled = false;
        var samuraiScript = ghostTowerInstance.GetComponent<SamuraiT>();
        if (samuraiScript != null) samuraiScript.enabled = false;

        SpriteRenderer sr = ghostTowerInstance.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1f, 1f, 1f, 0.5f);
    }

    private void UpdateGhostTowerPosition()
    {
        if (ghostTowerInstance == null) return;
        Collider2D slotCollider = Physics2D.OverlapCircle(transform.position, 0.2f, towerSlotLayer);

        if (slotCollider != null /* && !slotCollider.GetComponent<TowerSlot>().isOccupied */)
        {
            ghostTowerInstance.transform.position = slotCollider.transform.position;
            ghostTowerInstance.SetActive(true);
        }
        else
        {
            ghostTowerInstance.SetActive(false);
        }
    }

// --- NOVOS MÉTODOS PARA O FLIP ---
private void HandleFlip()
    {
        // Se o jogador está se movendo para a esquerda (input.x < 0) e ainda está virado para a direita...
        if (moveInput.x < 0 && isFacingRight)
        {
            Flip(); // ...vira para a esquerda.
        }
        // Se o jogador está se movendo para a direita (input.x > 0) e está virado para a esquerda...
        else if (moveInput.x > 0 && !isFacingRight)
        {
            Flip(); // ...vira para a direita (volta ao normal).
        }
    }

    private void Flip()
    {
        // Inverte o estado da direção
        isFacingRight = !isFacingRight;

        // Pega a escala atual, inverte o valor de X e aplica de volta.
        // Isso espelha o sprite horizontalmente.
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    // --- MÉTODOS DE MANA E MORTE (sem alterações) ---
    public void AddMana(float amount)
    {
        currentMana += amount;
        if (currentMana > maxMana) currentMana = maxMana;
        Debug.Log($"Jogador ganhou {amount} de mana. Mana atual: {currentMana}");
    }

    public void SpendMana(float amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;
        Debug.Log($"Jogador gastou {amount} de mana. Mana atual: {currentMana}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("O JOGADOR MORREU!");
        gameObject.SetActive(false);
    }
}