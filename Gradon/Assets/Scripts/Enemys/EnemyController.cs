// Enemy.cs (Versão de Depuração com "Sensores")

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

    protected Transform currentTarget;
    protected Rigidbody2D rb;
    protected int currentHealth;
    protected bool isDead = false;
    protected Vector2 moveDirection;
    private bool isFacingRight = true;
    [Header("Comportamento de Separação")]
    [SerializeField] private bool avoidStacking = true;
    [SerializeField] private float separationForce = 5f;
    [SerializeField] private float separationRadius = 1f;

    public event System.Action<Enemy> OnDeath;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // A vida é inicializada APENAS AQUI. Isso está correto.
        currentHealth = maxHealth;
        if (Totem.instance != null) { currentTarget = Totem.instance.transform; }
        else
        {
            Totem totem = FindFirstObjectByType<Totem>();
            if (totem != null) { currentTarget = totem.transform; }
            else { Debug.LogError("Nenhuma base (Totem) encontrada na cena!", this); }
        }
        UpdateHealthBar();
    }

    // ... (Seus métodos de movimento, Flip, etc. não precisam de mudança) ...
    public bool IsDead() { return isDead; }
    protected virtual void Update() { /* ... */ }
    private void FixedUpdate() { /* ... */ }
    protected abstract Vector2 HandleMovement();
    private void CalculateSeparationVector() { /* ... */ }
    protected void FlipTowardsTarget() { /* ... */ }
    private void Flip() { /* ... */ }


    // --- O PONTO CRÍTICO DA NOSSA INVESTIGAÇÃO ---
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // SENSOR 1: O comando de dano foi recebido? E com qual valor?
        Debug.Log($"'{gameObject.name}' recebeu o comando TakeDamage com {damage} de dano.");

        currentHealth -= damage;

        // SENSOR 2: Como a vida ficou após o cálculo?
        Debug.Log($"Vida de '{gameObject.name}' agora é: {currentHealth} / {maxHealth}");

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            // SENSOR 3: A condição de morte foi atingida?
            Debug.Log($"<color=red>'{gameObject.name}' atingiu 0 de vida! Chamando o método Die().</color>");
            Die();
        }
    }

    public void TakeDamage(float damage) { TakeDamage(Mathf.RoundToInt(damage)); }

    protected void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

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
        OnDeath?.Invoke(this);
        Destroy(gameObject, 0.1f);
    }
}