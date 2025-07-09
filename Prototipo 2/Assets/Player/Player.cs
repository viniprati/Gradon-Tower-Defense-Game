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

    [Header("Referências de Construção")]
    public List<GameObject> availableTowers;

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

        // Atualiza a UI com os valores iniciais de mana assim que o jogo começa.
        UpdateManaUI();
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
            return;
        }

        if (moveInput.magnitude > 1)
        {
            moveInput.Normalize();
        }

        // Aplica o movimento ao Rigidbody
        rb.linearVelocity = moveInput * moveSpeed;
    }

    // --- MÉTODOS PÚBLICOS PARA OS BOTÕES ---
    public void OnUpButtonPressed() { if (!isBuilding) moveInput.y = 1; }
    public void OnDownButtonPressed() { if (!isBuilding) moveInput.y = -1; }
    public void OnLeftButtonPressed()
    {
        if (isBuilding) CycleTowerSelection(-1);
        else { moveInput.x = -1; if (isFacingRight) Flip(); }
    }
    public void OnRightButtonPressed()
    {
        if (isBuilding) CycleTowerSelection(1);
        else { moveInput.x = 1; if (!isFacingRight) Flip(); }
    }
    public void OnVerticalButtonReleased() { moveInput.y = 0; }
    public void OnHorizontalButtonReleased() { moveInput.x = 0; }

    public void OnBuildActionButtonPressed()
    {
        if (inputCooldownTimer > 0) return;

        if (!isBuilding)
        {
            int towerCost = 10;
            if (currentMana >= towerCost) EnterBuildMode();
            else Debug.Log("Mana insuficiente para iniciar a construção!");
        }
        else
        {
            TryPlaceTower();
        }
    }

    // --- LÓGICA DE CONSTRUÇÃO ---
    private void TryPlaceTower()
    {
        if (ghostTowerInstance == null) return;

        GameObject towerToBuildPrefab = availableTowers[selectedTowerIndex];
        int towerCost = 10;

        if (currentMana >= towerCost)
        {
            SpendMana(towerCost);
            Instantiate(towerToBuildPrefab, ghostTowerInstance.transform.position, Quaternion.identity);
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
        moveInput = Vector2.zero;
        inputCooldownTimer = INPUT_COOLDOWN;
        selectedTowerIndex = 0;
        SpawnGhostTower();
    }

    private void ExitBuildMode()
    {
        isBuilding = false;
        inputCooldownTimer = INPUT_COOLDOWN;
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
        if (availableTowers == null || availableTowers.Count == 0) return;

        ghostTowerInstance = Instantiate(availableTowers[selectedTowerIndex]);
        ghostTowerInstance.name = "GHOST_TOWER";

        Collider2D ghostCollider = ghostTowerInstance.GetComponent<Collider2D>();
        if (ghostCollider != null) ghostCollider.enabled = false;

        var samuraiScript = ghostTowerInstance.GetComponent<SamuraiT>();
        if (samuraiScript != null) samuraiScript.enabled = false;

        SpriteRenderer sr = ghostTowerInstance.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1f, 1f, 1f, 0.5f);
    }

    private void UpdateGhostTowerPosition()
    {
        if (ghostTowerInstance != null)
        {
            ghostTowerInstance.transform.position = this.transform.position;
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

        // Avisa o UIManager para atualizar a barra de mana.
        UpdateManaUI();
    }

    public void SpendMana(float amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;

        // Avisa o UIManager para atualizar a barra de mana.
        UpdateManaUI();
    }

    private void UpdateManaUI()
    {
        // Verifica se o UIManager existe na cena antes de tentar usá-lo
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateManaUI(currentMana, maxMana);
        }
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