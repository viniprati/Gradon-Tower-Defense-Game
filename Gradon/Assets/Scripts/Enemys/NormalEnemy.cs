// NormalEnemy.cs
using UnityEngine;

public class NormalEnemy : Enemy
{
    [Header("Configuração Extra Normal Enemy")]
    [SerializeField] private int attackDamage = 10;
    // O attackRange agora vem da classe base, mas você pode sobrescrevê-lo no Inspector se quiser um valor diferente para este inimigo

    protected override void Start()
    {
        base.Start();
        health = 50; // vida fixa

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
        // Agora, a lógica de movimento e desaceleração é cuidada pelo método da classe base
        MoveTowardsTarget();
    }

    public override void Attack()
    {
        if (Totem.instance != null)
        {
            Totem.instance.TakeDamage(attackDamage);
        }
    }
}