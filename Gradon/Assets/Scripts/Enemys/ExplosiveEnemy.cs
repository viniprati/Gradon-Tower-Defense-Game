using UnityEngine;

public class GhoulController : Enemy // Herda da classe base
{
    [Header("Atributos de Ataque do Ghoul")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackRate = 1f; // 1 ataque por segundo
    [SerializeField] private float attackDamage = 20f;

    private float attackCooldown = 0f;

    // A l�gica de movimento do Ghoul � simplesmente avan�ar em dire��o ao alvo.
    protected override Vector2 HandleMovement()
    {
        // Se estivermos perto o suficiente para atacar, paramos de nos mover.
        if (currentTarget != null && Vector2.Distance(transform.position, currentTarget.position) <= attackRange)
        {
            return Vector2.zero; // Retorna um vetor zero para parar o movimento.
        }

        // Caso contr�rio, retorna a dire��o normal para o alvo.
        return moveDirection;
    }

    // O Update � usado para controlar a l�gica de ataque baseada em tempo.
    protected override void Update()
    {
        base.Update(); // Executa a l�gica do Update da classe base (virar o sprite, etc.)

        if (isDead || currentTarget == null) return;

        // L�gica de cooldown do ataque
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        // Verifica se pode atacar
        if (Vector2.Distance(transform.position, currentTarget.position) <= attackRange)
        {
            if (attackCooldown <= 0)
            {
                Attack();
                attackCooldown = 1f / attackRate;
            }
        }
    }

    private void Attack()
    {
        // Tenta encontrar um componente que possa receber dano no alvo.
        IDamageable targetHealth = currentTarget.GetComponent<IDamageable>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage);
            Debug.Log(gameObject.name + " atacou " + currentTarget.name);
        }
    }
}