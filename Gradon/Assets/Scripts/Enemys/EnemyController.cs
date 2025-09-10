// Enemy.cs

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Atributos Base")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected float speed = 2f;
    [Tooltip("Quantidade de mana que o jogador ganha ao derrotar este inimigo.")]
    [SerializeField] protected int manaOnKill = 10;

    [Header("Referências")]
    [SerializeField] protected Image healthBarFill;

    [Header("Comportamento de Separação")]
    [Tooltip("Ative para que os inimigos tentem não ficar uns em cima dos outros.")]
    [SerializeField] private bool avoidStacking = true;
    [SerializeField] private float separationForce = 5f;
    [SerializeField] private float separationRadius = 1f;

    // Evento para o WaveSpawner saber quando um inimigo morreu.
    public event System.Action<Enemy> OnDeath;

    // Variáveis internas protegidas para que classes filhas possam acessá-las
    protected Transform currentTarget;
    protected Rigidbody2D rb;
    protected int currentHealth;
    protected bool isDead = false;

    // Variáveis internas privadas
    private Vector2 moveDirection;
    private bool isFacingRight = true;

    /// <summary>
    /// Chamado pela Unity quando o objeto é criado.
    /// </summary>
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // Encontra o Totem como alvo
        if (Totem.instance != null) { currentTarget = Totem.instance.transform; }
        else { Debug.LogError("Nenhuma base (Totem) encontrada na cena!", this); }

        UpdateHealthBar();
    }

    /// <summary>
    /// Chamado a cada frame. Ideal para lógica que não envolve física.
    /// </summary>
    protected virtual void Update()
    {
        if (isDead || currentTarget == null)
        {
            if (rb != null) rb.velocity = Vector2.zero; // Para o movimento se estiver morto
            return;
        }

        // Calcula a direção para virar o sprite
        moveDirection = (currentTarget.position - transform.position).normalized;
        FlipTowardsTarget();
    }

    /// <summary>
    /// Chamado em um intervalo fixo. Ideal para toda a lógica de física.
    /// </summary>
    private void FixedUpdate()
    {
        if (isDead || currentTarget == null)
        {
            rb.velocity = Vector2.zero; // Garante que pare
            return;
        }

        // 1. Calcula o vetor de movimento principal em direção ao alvo
        Vector2 movementVector = (currentTarget.position - transform.position).normalized;

        // 2. Calcula um vetor opcional para evitar que os inimigos se empilhem
        Vector2 separationVector = avoidStacking ? CalculateSeparationVector() : Vector2.zero;

        // 3. Combina os vetores, normaliza para ter uma direção final, e aplica a velocidade
        Vector2 finalVelocity = (movementVector + (separationVector * separationForce)).normalized * speed;
        rb.velocity = finalVelocity;
    }

    /// <summary>
    /// Calcula uma força de repulsão baseada nos vizinhos próximos.
    /// </summary>
    private Vector2 CalculateSeparationVector()
    {
        Vector2 steer = Vector2.zero;
        int neighborsCount = 0;
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, separationRadius);

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == this.gameObject || !neighbor.CompareTag("Enemy")) continue;

            Vector2 difference = transform.position - neighbor.transform.position;
            steer += difference.normalized / (difference.magnitude + 0.01f); // Adiciona força inversamente proporcional à distância
            neighborsCount++;
        }

        if (neighborsCount > 0) { steer /= neighborsCount; }
        return steer;
    }

    /// <summary>
    /// Vira o sprite do inimigo para a esquerda ou direita, dependendo da direção do movimento.
    /// </summary>
    protected void FlipTowardsTarget()
    {
        if (moveDirection.x > 0 && !isFacingRight) Flip();
        else if (moveDirection.x < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    /// <summary>
    /// Método público para receber dano de fontes externas (como projéteis).
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        UpdateHealthBar();
        if (currentHealth <= 0) Die();
    }

    // Sobrecarga para aceitar dano do tipo float e convertê-lo para int.
    public void TakeDamage(float damage) { TakeDamage(Mathf.RoundToInt(damage)); }

    /// <summary>
    /// Atualiza a barra de vida visual do inimigo.
    /// </summary>
    protected void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    /// <summary>
    /// Lógica executada quando a vida do inimigo chega a zero.
    /// </summary>
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;

        if (Totem.instance != null)
        {
            Totem.instance.AddMana(manaOnKill);
        }

        // Dispara o evento para avisar ao WaveSpawner que este inimigo morreu.
        OnDeath?.Invoke(this);

        Destroy(gameObject, 0.1f);
    }

    // Método abstrato que força as classes filhas a implementar sua própria lógica, se necessário.
    // Como o movimento básico já está no FixedUpdate, podemos remover isso para simplificar.
    // protected abstract Vector2 HandleMovement(); 
}