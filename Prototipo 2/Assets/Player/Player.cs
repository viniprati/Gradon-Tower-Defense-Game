// PlayerController.cs (Versão Final para Construção Livre)
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
    // As variáveis 'towerSlotLayer' e 'buildModeUI' foram removidas, pois não são mais necessárias para esta lógica.

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
    }

    void Update()
    {
        if (inputCooldownTimer > 0)
        {
            inputCooldownTimer -= Time.deltaTime;
        }

        // Se estiver no modo de construção, a pré-visualização segue o jogador.
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
            // Checa se há mana suficiente ANTES de entrar no modo de construção
            int towerCost = 10; // Custo de exemplo, idealmente pegaria o custo da primeira torre
            if (currentMana >= towerCost)
            {
                EnterBuildMode();
            }
            else
            {
                Debug.Log("Mana insuficiente para iniciar a construção!");
            }
        }
        else
        {
            TryPlaceTower();
        }
    }

    // --- LÓGICA DE CONSTRUÇÃO SIMPLIFICADA ---
    private void TryPlaceTower()
    {
        if (ghostTowerInstance == null) return;

        GameObject towerToBuildPrefab = availableTowers[selectedTowerIndex];
        int towerCost = 10; // Custo de exemplo

        if (currentMana >= towerCost)
        {
            SpendMana(towerCost);
            // Constrói a torre na posição atual da pré-visualização (que é a posição do jogador).
            Instantiate(towerToBuildPrefab, ghostTowerInstance.transform.position, Quaternion.identity);
            ExitBuildMode();
        }
        else
        {
            Debug.Log("Mana insuficiente!");
            // O jogador permanece no modo de construção até ter mana ou cancelar
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