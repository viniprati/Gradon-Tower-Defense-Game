using UnityEngine;

public class NormalEnemy : Enemy
{
    [Header("Configuração Extra")]
    [SerializeField] private float speedMultiplier = 1.5f; // velocidade aumentada
    [SerializeField] private int maxHealthExtra = 100;      // vida aumentada
    [SerializeField] private int attackDamage = 10;         // dano ao Totem

    private int currentHealthExtra;

    protected override void Start()
    {
        base.Start();
        currentHealthExtra = maxHealthExtra;
    }

    void Update()
    {
        if (Totem.instance == null || IsDead) return;

        float distance = Vector3.Distance(transform.position, Totem.instance.transform.position);

        if (distance <= 1f)
        {
            rb.linearVelocity = Vector2.zero; // para de se mover ao atacar
            Attack();
        }
        else
        {
            MoveTowardsTotem();
        }
    }

    private void MoveTowardsTotem()
    {
        Vector3 direction = (Totem.instance.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * speed * speedMultiplier;
    }

    public override void Attack()
    {
        if (Totem.instance != null)
        {
            Totem.instance.TakeDamage(attackDamage);
            Debug.Log("NormalEnemy atacou o Totem!");
        }
    }

    // Função para receber dano das torres
    public void TakeDamage(int damage)
    {
        currentHealthExtra -= damage;

        if (currentHealthExtra <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignora colisão com torres para não desacelerar
        if (collision.gameObject.CompareTag("Tower"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
}
