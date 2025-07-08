// PlayerController.cs (Versão Corrigida para Tablet com Construção)
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

    [Header("Referências de Construção")]
    public List<GameObject> availableTowers;
    public LayerMask towerSlotLayer;
    [Tooltip("Arraste aqui um painel de UI que só aparece no modo de construção, se tiver um.")]
    public GameObject buildModeUI;

    // --- Variáveis Internas ---
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isBuilding = false;
    private bool isFacingRight = true;
    private int selectedTowerIndex = 0;
    private GameObject ghostTowerInstance;

    private float inputCooldownTimer = 0f;
    private const float INPUT_COOLDOWN = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentMana = maxMana;
        moveInput = Vector2.zero;
        if (buildModeUI != null) buildModeUI.SetActive(false);
    }

    void Update()
    {
        if (inputCooldownTimer > 0)
        {
            inputCooldownTimer -= Time.deltaTime;
        }

        if (isBuilding)
        {
            UpdateGhostTowerPosition();
        }
    }

    void FixedUpdate()
    {
        // Se estivermos no modo de construção, a velocidade é zero.
        if (isBuilding)
        {
            rb.linearVelocity = Vector2.zero;
            return; // Sai do método para não aplicar o movimento abaixo
        }

        // Se NÃO estiver construindo, aplica o movimento normalmente.
        if (moveInput.magnitude > 1)
        {
            moveInput.Normalize();
        }
        // CORREÇÃO: A propriedade correta do Rigidbody2D é 'velocity'.
        rb.linearVelocity = moveInput * moveSpeed;
    }

    // --- MÉTODOS PÚBLICOS PARA OS BOTÕES ---

    public void OnUpButtonPressed() { if (!isBuilding) moveInput.y = 1; }
    public void OnDownButtonPressed() { if (!isBuilding) moveInput.y = -1; }

    public void OnLeftButtonPressed()
    {
        if (isBuilding)
        {
            CycleTowerSelection(-1);
        }
        else
        {
            moveInput.x = -1;
            if (isFacingRight) Flip();
        }
    }

    public void OnRightButtonPressed()
    {
        if (isBuilding)
        {
            CycleTowerSelection(1);
        }
        else
        {
            moveInput.x = 1;
            if (!isFacingRight) Flip();
        }
    }

    public void OnVerticalButtonReleased() { moveInput.y = 0; }
    public void OnHorizontalButtonReleased() { moveInput.x = 0; }

    public void OnBuildActionButtonPressed()
    {
        if (inputCooldownTimer > 0) return;

        if (!isBuilding)
        {
            EnterBuildMode();
        }
        else
        {
            TryPlaceTower();
        }
    }

    // --- LÓGICA DE CONSTRUÇÃO ---

    private void TryPlaceTower()
    {
        if (ghostTowerInstance == null || !ghostTowerInstance.activeSelf)
        {
            Debug.Log("Não é possível construir aqui!");
            return;
        }

        GameObject towerToBuildPrefab = availableTowers[selectedTowerIndex];
        int towerCost = 10; // Custo de exemplo

        if (currentMana >= towerCost)
        {
            SpendMana(towerCost);
            Vector3 buildPosition = ghostTowerInstance.transform.position;
            Instantiate(towerToBuildPrefab, buildPosition, Quaternion.identity);
            ExitBuildMode();
        }
        else
        {
            Debug.Log("Mana insuficiente!");
        }
    }

    private void EnterBuildMode()
    {
        isBuilding = true;
        moveInput = Vector2.zero; // Reseta o input para parar o movimento
        inputCooldownTimer = INPUT_COOLDOWN;
        if (buildModeUI != null) buildModeUI.SetActive(true);
        selectedTowerIndex = 0;
        SpawnGhostTower();
    }

    private void ExitBuildMode()
    {
        isBuilding = false;
        inputCooldownTimer = INPUT_COOLDOWN;
        if (buildModeUI != null) buildModeUI.SetActive(false);
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
    }

    private void SpawnGhostTower()
    {
        if (availableTowers.Count == 0) return;
        ghostTowerInstance = Instantiate(availableTowers[selectedTowerIndex]);
        ghostTowerInstance.name = "GHOST_TOWER";
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

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }


// --- MÉTODOS DE MANA E MORTE ---
public void AddMana(float amount)
    {
        currentMana += amount;
        if (currentMana > maxMana) currentMana = maxMana;
    }

    public void SpendMana(float amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;
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