using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Configuração Extra")]
    [SerializeField] private float speedMultiplier = 1.2f; // aumenta a velocidade
    [SerializeField] private int maxHealthExtra = 80;      // vida do inimigo

    public GameObject projectile;
    public float fireRate = 1f;

    private float fireCooldown = 0f;
    private int currentHealthExtra;

    protected override void Start()
    {
        base.Start();
        currentHealthExtra = maxHealthExtra;
    }

    void Update()
    {
        if (Totem.instance == null || IsDead) return;

        // Movimenta em direção ao Totem
        Vector3 direction = (Totem.instance.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * speed * speedMultiplier;

        // Ataque à distância
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Attack();
            fireCooldown = fireRate;
        }
    }

    public override void Attack()
    {
        if (projectile != null)
        {
            Instantiate(projectile, transform.position, Quaternion.identity);
            Debug.Log("RangedEnemy atirou um projétil!");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealthExtra -= damage;
        if (currentHealthExtra <= 0)
        {
            Die();
        }
    }
}
