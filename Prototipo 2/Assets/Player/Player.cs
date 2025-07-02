using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidade do jogador
    private Rigidbody2D rb;
    private Vector2 moveInput;

    // Variável para controlar se estamos no modo de construção
    private bool isBuilding = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Se NÃO estivermos no modo de construção, o player pode se mover.
        if (!isBuilding)
        {
            // Captura input de movimento
            moveInput.x = Input.GetAxisRaw("Horizontal"); // A e D
            moveInput.y = Input.GetAxisRaw("Vertical");   // W e S
            moveInput.Normalize(); // Impede movimento mais rápido na diagonal
        }
        else
        {
            // Se estiver no modo de construção, o player fica parado.
            moveInput = Vector2.zero;
        }

        // --- Lógica de Construção ---
        // Pressionar ESPAÇO alterna o modo de construção
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isBuilding = !isBuilding; // Inverte o valor (true vira false, false vira true)

            if (isBuilding)
            {
                // Avisa o BuildManager para entrar no modo de construção
                BuildManager.instance.EnterBuildMode();
            }
            else
            {
                // Se já estávamos construindo, agora vamos tentar colocar a torre
                BuildManager.instance.TryPlaceTower(transform.position);
                // E saímos do modo de construção
                BuildManager.instance.ExitBuildMode();
            }
        }

        // Se estivermos no modo de construção, A e D trocam a torre selecionada
        if (isBuilding)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                BuildManager.instance.SelectPreviousTower();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                BuildManager.instance.SelectNextTower();
            }
        }
    }

    void FixedUpdate()
    {
        // Aplica o movimento no Rigidbody
        rb.linearVelocity = moveInput * moveSpeed;
    }
}

// tá indo, devagar mas tá indo