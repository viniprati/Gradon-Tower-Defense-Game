// PlayerController.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configurações do Jogador")]
    public float moveSpeed = 5f;

    [Header("Recursos do Jogador")]
    public float maxMana = 100f;
    public float currentMana;

    // --- Variáveis Internas ---
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isBuilding = false;

    // --- NOVA VARIÁVEL PARA O FLIP ---
    private bool isFacingRight = true; // Assumimos que o sprite começa virado para a direita

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentMana = maxMana;
    }

    void Update()
    {
        // Se NÃO estivermos no modo de construção, o player pode se mover.
        if (!isBuilding)
        {
            // --- CAPTURA DE INPUT DE MOVIMENTO ---
            moveInput.x = Input.GetAxisRaw("Horizontal"); // A e D
            moveInput.y = Input.GetAxisRaw("Vertical");   // W e S
            moveInput.Normalize();

            // --- NOVA LÓGICA DE FLIP ---
            HandleFlip();
        }
        else
        {
            // Se estiver no modo de construção, o player fica parado.
            moveInput = Vector2.zero;
        }

        // --- Lógica de Construção ---
        // (seu código de construção permanece aqui, sem alterações)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isBuilding = !isBuilding;

            if (isBuilding)
            {
                //BuildManager.instance.EnterBuildMode();
            }
            else
            {
                //BuildManager.instance.TryPlaceTower(transform.position);
                //BuildManager.instance.ExitBuildMode();
            }
        }

        if (isBuilding)
        {
            if (Input.GetKeyDown(KeyCode.A)) { /* BuildManager.instance.SelectPreviousTower(); */ }
            if (Input.GetKeyDown(KeyCode.D)) { /* BuildManager.instance.SelectNextTower(); */ }
            if (Input.GetKeyDown(KeyCode.S))
            {
                isBuilding = false;
                //BuildManager.instance.ExitBuildMode();
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    // --- NOVO MÉTODO PARA CONTROLAR O FLIP ---
    private void HandleFlip()
    {
        // Se o jogador está se movendo para a esquerda (input.x < 0) e está virado para a direita...
        if (moveInput.x < 0 && isFacingRight)
        {
            Flip(); // ...vira para a esquerda.
        }
        // Se o jogador está se movendo para a direita (input.x > 0) e está virado para a esquerda...
        else if (moveInput.x > 0 && !isFacingRight)
        {
            Flip(); // ...vira para a direita.
        }
    }

    // --- NOVO MÉTODO QUE FAZ A MÁGICA DO FLIP ---
    private void Flip()
    {
        // Inverte o estado da direção
        isFacingRight = !isFacingRight;

        // Inverte a escala do objeto no eixo X para espelhar o sprite
        Vector3 aEscala = transform.localScale;
        aEscala.x *= -1; // Multiplica por -1 (ex: 1 vira -1, -1 vira 1)
        transform.localScale = aEscala;
    }

    // --- Outros Métodos (Mana, Morte) ---
    public void AddMana(float amount)
    {
        currentMana += amount;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
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