// Enemy.cs

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Atributos Base")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected int manaOnKill = 15;

    [Tooltip("Dano que o inimigo causa ao colidir com o Totem.")]
    [SerializeField] protected int damageToTotem = 50;

    // Vari�veis protegidas para que as classes filhas possam acess�-las
    protected Transform target;
    protected Rigidbody2D rb;
    protected float currentHealth;
    protected bool isDead = false;

    // Evento para o WaveSpawner saber que o inimigo morreu
    public event System.Action<Enemy> OnDeath;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // � importante que seja Dynamic para a colis�o f�sica funcionar
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0; // Para n�o cair

        currentHealth = maxHealth;
        if (Totem.instance != null) { target = Totem.instance.transform; }
    }

    /// <summary>
    /// A l�gica de movimento agora est� em FixedUpdate para consist�ncia com a f�sica.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        if (isDead || target == null) return;

        // Move o Rigidbody em dire��o ao alvo
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    /// <summary>
    /// Chamado automaticamente pela Unity quando o colisor deste inimigo
    /// bate fisicamente em outro colisor "s�lido".
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o objeto em que batemos tem a tag "Totem".
        if (collision.gameObject.CompareTag("Totem"))
        {
            // Tenta pegar o componente 'Totem' do objeto atingido.
            Totem totem = collision.gameObject.GetComponent<Totem>();
            if (totem != null)
            {
                // Causa dano ao Totem.
                totem.TakeDamage(damageToTotem);
            }

            // O inimigo se sacrifica no ataque, chamando seu pr�prio m�todo Die().
            Die();
        }
    }

    /// <summary>
    /// M�todo p�blico para receber dano de fontes externas (como proj�teis).
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// M�todo p�blico para que outros scripts possam verificar se o inimigo est� morto.
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }

    /// <summary>
    /// L�gica executada quando a vida do inimigo chega a zero ou ele ataca o totem.
    /// </summary>
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke(this);
        if (Totem.instance != null) { Totem.instance.AddMana(manaOnKill); }
        Destroy(gameObject);
    }
}