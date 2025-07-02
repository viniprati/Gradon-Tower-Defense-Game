// Dentro do script MeleeTower.cs
using UnityEngine;

public class SamuraiT : MonoBehaviour
{
    [Header("Atributos da Torre")]
    public float attackRange = 2f;
    public float attackRate = 1f; // Ataques por segundo
    public int damage = 10;

    private float attackCooldown = 0f;

    void Update()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            Attack();
            attackCooldown = 1f / attackRate;
        }
    }

    void Attack()
    {
        // Encontra TODOS os colisores dentro do raio de ataque
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (var col in collidersInRange)
        {
            // Verifica se o colisor pertence a um inimigo
            if (col.CompareTag("Enemy"))
            {
                Debug.Log("Torre Samurai atingiu " + col.name);
                // Aqui você chamaria uma função no script do inimigo para dar dano
                // Ex: col.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
        }
    }

    // Desenha o raio de ataque no editor para facilitar o debug
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}