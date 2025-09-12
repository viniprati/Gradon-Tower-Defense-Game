using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Configuração Extra")]
    [SerializeField] private float speedMultiplier = 1.2f;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float attackRange = 5f;

    private float fireCooldown = 0f;

    protected override void Start()
    {
        base.Start();
        health *= 2; // vida extra: torre precisa de dois disparos
    }

    void Update()
    {
        if (Totem.instance == null || IsDead) return;

        float distance = Vector3.Distance(transform.position, Totem.instance.transform.position);

        if (distance > attackRange)
        {
            Vector3 direction = (Totem.instance.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * speed * speedMultiplier;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                Attack();
                fireCooldown = fireRate;
            }
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
}
