// GhoulController.cs (Corrigido e integrado)

using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GhoulController : EnemyController // Herda da classe base 'Enemy'
{
    [Header("Atributos de Ataque do Ghoul")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float attackDamage = 20f;

    private float attackCooldown = 0f;

    // Sobrescrevemos o Update da classe base para adicionar a lógica de ataque
    protected override void Update()
    {
        if (isDead || currentTarget == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Verifica a distância até o alvo
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        // Se estiver longe, continua se movendo (chama a lógica da classe base)
        if (distanceToTarget > attackRange)
        {
            base.Update(); // Executa o movimento do Enemy.cs
        }
        // Se estiver perto o suficiente, para de se mover e ataca
        else
        {
            rb.velocity = Vector2.zero; // Para o movimento

            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0)
            {
                Attack();
                attackCooldown = 1f / attackRate;
            }
        }
    }

    private void Attack()
    {
        IDamageable targetHealth = currentTarget.GetComponent<IDamageable>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage);
            Debug.Log(gameObject.name + " atacou " + currentTarget.name);
        }
    }
}