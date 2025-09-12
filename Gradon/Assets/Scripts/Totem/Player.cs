// PlayerController.cs
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IDamageable
{
    // --- ESTADOS DO JOGADOR ---
    private enum PlayerState { Moving, Building }
    private PlayerState currentState = PlayerState.Moving;

    [Header("Configurações de Movimento")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Recursos do Jogador")]
    [SerializeField] private float maxMana = 100f;
    public float currentMana { get; private set; }

    [Header("Atributos de Combate")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Referências de Construção")]
    [SerializeField] private List<GameObject> availableTowers;
    [SerializeField] private Button buildButton;
    [SerializeField] private BuildUIController buildUIController;

    // --- Variáveis Internas ---
    private Rigidbody2D rb;
    private Vector2 moveInput; // Esta variável será manipulada por ambos os sistemas de input
    private bool isFacingRight = true;
    private int selectedTowerIndex = 0;
    private float inputCooldownTimer = 0f;
    private const float INPUT_COOLDOWN = 0.2f;

    #region Ciclo de Vida e Eventos Unity
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentMana = maxMana;
        currentHealth = maxHealth;
        UpdateManaUI();
        HandleManaChange(currentMana);
    }

    void OnEnable()
    {
        GameManager.OnManaChanged += HandleManaChange;
    }

    void OnDisable()
    {
        GameManager.OnManaChanged -= HandleManaChange;
    }

    void Update()
    {
        // A ordem aqui é importante. Primeiro processamos o teclado.
        // Os botões da UI, sendo baseados em eventos, podem modificar 'moveInput' a qualquer momento.
        ProcessKeyboardInput();

        if (inputCooldownTimer > 0)
        {
            inputCooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        // A física é aplicada aqui, usando o valor de 'moveInput' que foi definido
        // pelo teclado OU pelos botões da UI.
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }
    #endregion

    #region Lógica de Input Híbrido

    // --- MÉTODOS PÚBLICOS PARA OS BOTÕES DA UI ---
    // Estes métodos definem o estado de 'moveInput'.
    public void OnUpButtonPressed() { if (currentState == PlayerState.Moving) moveInput.y = 1; }
    public void OnDownButtonPressed() { if (currentState == PlayerState.Moving) moveInput.y = -1; }
    public void OnVerticalButtonReleased() { if (moveInput.y != 0) moveInput.y = 0; }

    public void OnLeftButtonPressed()
    {
        if (currentState == PlayerState.Building) CycleTowerSelection(-1);
        else { moveInput.x = -1; CheckFlip(); }
    }
    public void OnRightButtonPressed()
    {
        if (currentState == PlayerState.Building) CycleTowerSelection(1);
        else { moveInput.x = 1; CheckFlip(); }
    }
    public void OnHorizontalButtonReleased() { if (moveInput.x != 0) moveInput.x = 0; }

    public void OnBuildActionButtonPressed()
    {
        if (inputCooldownTimer > 0) return;
        inputCooldownTimer = INPUT_COOLDOWN;

        if (currentState == PlayerState.Moving) EnterBuildMode();
        else TryPlaceTower();
    }

    // --- LÓGICA DO TECLADO ---
    private void ProcessKeyboardInput()
    {
        // Só processa o input do teclado se estivermos no editor ou em build de desktop
#if UNITY_EDITOR || UNITY_STANDALONE

        // --- LÓGICA DE DETECÇÃO DE ATIVIDADE DO TECLADO ---
        // Verificamos se ALGUMA tecla de movimento está sendo usada.
        bool isKeyboardMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ||
                                  Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ||
                                  Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ||
                                  Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        // --- LÓGICA DE APLICAÇÃO DE INPUT ---
        switch (currentState)
        {
            case PlayerState.Moving:
                // Se o teclado está ativamente sendo usado para movimento...
                if (isKeyboardMoving)
                {
                    // ...então o teclado assume o controle total e define o moveInput.
                    // Usamos GetAxisRaw porque ele lida com "pressionar" e "soltar" perfeitamente.
                    moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                }
                // Se o teclado estiver ocioso (isKeyboardMoving é falso), o moveInput NÃO é tocado.
                // Isso permite que os valores definidos pelos botões da UI persistam.

                CheckFlip(); // Verifica o flip baseado no 'moveInput' final

                // Ação de construir com o teclado
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    OnBuildActionButtonPressed();
                }
                break;

            case PlayerState.Building:
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) CycleTowerSelection(-1);
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) CycleTowerSelection(1);
                if (Input.GetKeyDown(KeyCode.Space)) OnBuildActionButtonPressed();
                break;
        }
#endif
    }
    #endregion

    #region Lógica de Construção
    private void EnterBuildMode()
    {
        // ... (código existente, está correto)
        if (availableTowers == null || availableTowers.Count == 0 || buildUIController == null) return;
        currentState = PlayerState.Building;
        moveInput = Vector2.zero; // PARA o movimento ao entrar no modo de construção.
        selectedTowerIndex = 0;
        buildUIController.ShowSelection(availableTowers, selectedTowerIndex);
    }

    private void TryPlaceTower()
    {
        // ... (código existente, está correto)
        GameObject towerPrefab = availableTowers[selectedTowerIndex];
        TowerBase towerInfo = towerPrefab.GetComponent<TowerBase>();
        if (currentMana >= towerInfo.cost)
        {
            SpendMana(towerInfo.cost);
            Instantiate(towerPrefab, transform.position, Quaternion.identity);
            ExitBuildMode();
        }
    }

    private void ExitBuildMode()
    {
        // ... (código existente, está correto)
        currentState = PlayerState.Moving;
        if (buildUIController != null) buildUIController.HideSelection();
    }

    public void CycleTowerSelection(int direction)
    {
        // ... (código existente, está correto)
        if (currentState != PlayerState.Building || inputCooldownTimer > 0) return;
        inputCooldownTimer = INPUT_COOLDOWN;
        selectedTowerIndex = (selectedTowerIndex + direction + availableTowers.Count) % availableTowers.Count;
        if (buildUIController != null) buildUIController.UpdateSelection(selectedTowerIndex);
    }
    #endregion

    #region Combate, Mana e Utilitários
    // --- O restante do seu código (AddMana, SpendMana, TakeDamage, etc.) pode ser colado aqui. ---
    // --- Ele já está correto e não precisa de alterações. ---
    public void AddMana(float amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        UpdateManaUI();
    }

    private void SpendMana(float amount)
    {
        currentMana -= amount;
        UpdateManaUI();
    }

    private void UpdateManaUI()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.UpdateManaUI(currentMana);
        }
    }

    private void HandleManaChange(float newMana)
    {
        if (buildButton == null) return;
        bool canBuildAnything = false;
        foreach (var towerPrefab in availableTowers)
        {
            if (newMana >= towerPrefab.GetComponent<TowerBase>().cost)
            {
                canBuildAnything = true;
                break;
            }
        }
        buildButton.interactable = canBuildAnything;
    }

    private void CheckFlip()
    {
        // Verifica o 'moveInput' em vez de ler o eixo novamente.
        if (moveInput.x > 0 && !isFacingRight) Flip();
        else if (moveInput.x < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
       // transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        Debug.Log($"Player tomou {damage} de dano. Vida restante: {currentHealth}");
        if (currentHealth <= 0) Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(9999);
        }
    }

    // ADICIONE ESTE NOVO MÉTODO PARA CUIDAR DE COLISÕES "TRIGGER"
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se o objeto que entrou no nosso trigger tem a tag "Enemy"...
        if (other.CompareTag("Enemy"))
        {
            // ...causa dano massivo para garantir a morte.
            TakeDamage(9999);

            // Opcional, mas recomendado: Destruir o inimigo que tocou o jogador
            // para que ele não continue causando dano a cada frame.
            // Isso assume que o inimigo tem um método Die() ou pode ser destruído.
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead) // Adicione um método IsDead() ao seu EnemyBase
            {
                enemy.TakeDamage(9999); // Ou chame enemy.Die() diretamente
            }
        }
    }

    private void Die()
    {
        Debug.Log("O JOGADOR MORREU!");
        if (GameManager.instance != null) GameManager.instance.HandleGameOver();
        gameObject.SetActive(false);
    }
    #endregion
}