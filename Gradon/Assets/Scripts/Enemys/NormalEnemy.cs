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
        health *= 2; // torre precisa de dois tiros
        individualSpeed = speed * speedMultiplier;
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
