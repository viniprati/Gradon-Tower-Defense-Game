using UnityEngine;

public class NormalEnemy : Enemy
{
    [Header("Configuração Extra")]
    [SerializeField] private float speedMultiplier = 1.5f; // aumenta a velocidade
    [SerializeField] private int attackDamage = 10;         // dano ao Totem
    [SerializeField] private float attackRange = 1f;        // distância para atacar

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
            // mover em direção ao Totem
            Vector3 direction = (Totem.instance.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * speed * speedMultiplier;
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // para se mover ao atacar
            Attack();
        }
    }

    public override void Attack()
    {
        if (Totem.instance != null)
        {
            Totem.instance.TakeDamage(attackDamage);
            Debug.Log("NormalEnemy atacou o Totem!");
        }
    }
}
