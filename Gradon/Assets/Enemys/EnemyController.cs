// EnemyBase.cs
using UnityEngine;
using UnityEngine.UI; // Para a barra de vida

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Atributos Base")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected int scoreValue = 10;

    [Header("Referências")]
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected GameObject coinPrefab; // Prefab da moeda a ser dropada
    [SerializeField] protected Image healthBarFill; // Imagem com Fill Method = Filled

    // --- Variáveis Internas ---
    protected Rigidbody2D rb;
    protected float currentHealth;
    protected bool isDead = false;
    protected Vector2 moveDirection;
    private bool isFacingRight = true;

    [Header("Comportamento de Separação")]
    [Tooltip("Se marcado, inimigos tentarão não se sobrepor.")]
    [SerializeField] private bool avoidStacking = true;
    [Tooltip("O quão forte os inimigos se repelem.")]
    [SerializeField] private float separationForce = 5f;
    [Tooltip("O raio no qual um inimigo detecta outros para se separar.")]
    [SerializeField] private float separationRadius = 1f;

    // O 'virtual' permite que classes filhas sobrescrevam este método se precisarem
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // Tenta encontrar o jogador automaticamente
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Jogador não encontrado! Inimigos não funcionarão corretamente.", this);
        }

        UpdateHealthBar();
    }

    // Adicione este método público para que outros scripts possam verificar se o inimigo está morto.
    public bool IsDead()
    {
        return isDead;
    }

    // Update é usado para decisões e lógicas que não envolvem física
    protected virtual void Update()
    {
        if (isDead || playerTransform == null || !playerTransform.gameObject.activeInHierarchy)
        {
            // Se o inimigo está morto ou o player não existe mais, para de se mover
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        // Determina a direção do movimento e vira o sprite
        moveDirection = (playerTransform.position - transform.position).normalized;
        FlipTowardsPlayer();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero; // Garante que o inimigo pare ao morrer
            return;
        }

        // A lógica de movimento agora será composta
        Vector2 finalVelocity = Vector2.zero;

        // 1. Calcula o vetor de movimento principal (definido pelas classes filhas)
        Vector2 movementVector = HandleMovement();

        // 2. Calcula o vetor de separação (se habilitado)
        Vector2 separationVector = Vector2.zero;
        if (avoidStacking)
        {
            separationVector = CalculateSeparationVector();
        }

        // 3. Combina os vetores
        // O movimento principal tem peso 1, a separação tem um peso definido por 'separationForce'.
        finalVelocity = (movementVector + (separationVector * separationForce)).normalized * speed;

        // 4. Aplica a velocidade final ao Rigidbody
        rb.linearVelocity = finalVelocity;
    }

    // Método abstrato: FORÇA as classes filhas a implementarem sua própria lógica de movimento
    // MODIFICAÇÃO: HandleMovement agora retorna um vetor de direção
    protected abstract Vector2 HandleMovement();

    // NOVO MÉTODO: Calcula a força de repulsão dos vizinhos
    private Vector2 CalculateSeparationVector()
    {
        Vector2 steer = Vector2.zero;
        int neighborsCount = 0;

        // Encontra todos os outros inimigos dentro do raio de separação
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, separationRadius);

        foreach (var neighbor in neighbors)
        {
            // Ignora a si mesmo
            if (neighbor.gameObject == this.gameObject) continue;

            // Verifica se o vizinho também é um inimigo
            if (neighbor.CompareTag("Enemy"))
            {
                // Calcula um vetor apontando para longe do vizinho
                Vector2 difference = transform.position - neighbor.transform.position;
                // A força é maior quanto mais perto o vizinho está
                steer += difference.normalized / (difference.magnitude + 0.01f); // Adiciona 0.01f para evitar divisão por zero
                neighborsCount++;
            }
        }

        if (neighborsCount > 0)
        {
            // Tira a média do vetor de repulsão
            steer /= neighborsCount;
        }

        return steer;
    }
    // Vira o sprite do inimigo para encarar o jogador
    protected void FlipTowardsPlayer()
    {
        if (moveDirection.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveDirection.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }


    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    // 'virtual' para que o inimigo explosivo possa adicionar sua própria lógica
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        // Desativa o colisor para não interagir mais
        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero; // Para o movimento imediatamente

        // Dá score ao jogador através do GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(scoreValue);
        }

        // Dropa a moeda
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        // Destrói o objeto após um pequeno delay para efeitos visuais
        Destroy(gameObject, 0.1f);
    }
}