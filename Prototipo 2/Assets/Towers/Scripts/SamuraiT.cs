// SamuraiT.cs (Versão Corrigida)
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
        // Reduz o tempo de espera para o próximo ataque
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }

        // Se o tempo de espera acabou, ataca
        if (attackCooldown <= 0f)
        {
            Attack();
            // Reseta o tempo de espera
            attackCooldown = 1f / attackRate;
        }
    }

    void Attack()
    {
        // Encontra TODOS os colisores dentro do raio de ataque
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, attackRange);

        // Itera sobre cada colisor encontrado
        foreach (var col in collidersInRange)
        {
            // Verifica se o colisor pertence a um objeto com a tag "Enemy"
            if (col.CompareTag("Enemy"))
            {
                // --- A PARTE QUE FALTAVA ---

                // 1. Tenta pegar o script "EnemyController" do objeto que colidiu
                EnemyController enemy = col.GetComponent<EnemyController>();

                // 2. Se o script foi encontrado (não é nulo), aplica o dano
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    Debug.Log("Torre Samurai atingiu " + col.name + " e causou " + damage + " de dano.");
                }
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