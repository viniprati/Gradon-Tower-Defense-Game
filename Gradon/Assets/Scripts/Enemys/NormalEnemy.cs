using UnityEngine;

public class NormalEnemy : Enemy
{
    [Header("Configuração Extra")]
    [SerializeField] private float speedMultiplier = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 1f;

    private float individualSpeed;

    protected override void Start()
    {
        base.Start();
        health = 50; // vida fixa
        individualSpeed = speed * speedMultiplier;

        // Ignora colisão com o Totem e torres
        if (Totem.instance != null)
        {
            Collider2D totemCol = Totem.instance.GetComponent<Collider2D>();
            if (totemCol != null)
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), totemCol);
        }

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        foreach (var tower in towers)
        {
            Collider2D col = tower.GetComponent<Collider2D>();
            if (col != null)
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col);
        }
    }

    void Update()
    {
        if (Totem.instance == null || IsDead) return;

        float distance = Vector3.Distance(transform.position, Totem.instance.transform.position);

        if (distance > attackRange)
        {
            Vector3 direction = (Totem.instance.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * individualSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            Attack();
        }
    }

    public override void Attack()
    {
        if (Totem.instance != null)
        {
            Totem.instance.TakeDamage(attackDamage);
        }
    }
}
