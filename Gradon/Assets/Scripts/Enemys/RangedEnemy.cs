// RangedEnemy.cs
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RangedEnemy : Enemy
{
    [Header("Configuração Extra Ranged Enemy")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private float fireRate = 1f;
    // O attackRange agora vem da classe base, mas você pode sobrescrevê-lo no Inspector se quiser um valor diferente para este inimigo

    private float fireCooldown = 0f;

    protected override void Start()
    {
        base.Start();
        health *= 2; // vida extra: torre precisa de dois disparos
    }

    void Update()
    {
        // Agora, a lógica de movimento e desaceleração é cuidada pelo método da classe base
        MoveTowardsTarget();

        // A lógica de ataque baseada em cooldown permanece aqui, dentro do alcance de ataque
        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
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