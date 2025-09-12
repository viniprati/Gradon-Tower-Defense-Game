// NormalEnemy.cs (CORRIGIDO)
using UnityEngine;

public class NormalEnemy : Enemy
{
    [Header("Configurações do Normal Enemy")]
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackRate = 1f;
    private float attackCooldown = 0f;

    protected override void Update()
    {
        base.Update();
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public override void Attack()
    {
        if (attackCooldown <= 0 && Totem.instance != null && !Totem.instance.IsDestroyed)
        {
            Totem.instance.TakeDamage(attackDamage);
            attackCooldown = attackRate;
        }
    }
}