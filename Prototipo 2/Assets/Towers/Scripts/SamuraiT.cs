// SamuraiT.cs (Versão Final e Funcional)
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

    // --- MÉTODO Attack() MODIFICADO ---
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
                // --- MUDANÇA IMPLEMENTADA AQUI ---

                // 1. Tenta pegar a referência do script "EnemyController" do objeto que entrou no raio.
                EnemyController enemyScript = col.GetComponent<EnemyController>();

                // 2. Verifica se a referência foi encontrada (para evitar erros).
                if (enemyScript != null)
                {
                    // 3. Se encontrou, chama a função pública "TakeDamage" do inimigo e passa o dano da torre.
                    enemyScript.TakeDamage(damage);

                    // O Debug.Log agora confirma que o dano foi enviado com sucesso.
                    Debug.Log("Torre Samurai atacou " + col.name + " e causou " + damage + " de dano.");
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