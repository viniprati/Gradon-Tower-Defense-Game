// SamuraiT.cs (Versão Refeita - Confiável)
using UnityEngine;

public class SamuraiT : MonoBehaviour
{
    [Header("Atributos da Torre Samurai")]
    [Tooltip("O raio de alcance do corte da torre.")]
    [SerializeField] private float attackRange = 2f;

    [Tooltip("Quantas vezes por segundo a torre aplica o dano em área.")]
    [SerializeField] private float attackRate = 1.5f; // Ex: 1.5 ataques por segundo

    [Tooltip("O dano de cada 'pulso' de ataque.")]
    [SerializeField] private int damage = 10;

    // Variável interna para controlar o tempo entre os ataques
    private float attackCooldown = 0f;

    void Update()
    {
        // Reduz o tempo de espera para o próximo ataque
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }

        // Se o tempo de espera acabou, tenta atacar.
        if (attackCooldown <= 0f)
        {
            Attack();
            // Reseta o tempo de espera baseado na cadência de tiro.
            attackCooldown = 1f / attackRate;
        }
    }

    void Attack()
    {
        // 1. Encontra TODOS os colisores de objetos dentro do raio de ataque.
        // É importante especificar a Layer "Enemies" para otimização, se você a tiver.
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, attackRange);

        bool hasHitEnemy = false; // Para saber se acertamos alguém neste ataque

        // 2. Itera sobre cada colisor encontrado.
        foreach (var col in collidersInRange)
        {
            // 3. Verifica se o objeto tem a tag "Enemy".
            if (col.CompareTag("Enemy"))
            {
                // 4. Tenta pegar o script EnemyController do objeto.
                EnemyController enemy = col.GetComponent<EnemyController>();

                // 5. Se o script foi encontrado, aplica o dano.
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    hasHitEnemy = true; // Marcamos que um inimigo foi atingido
                }
            }
        }

        // Opcional: Log para depuração
        if (hasHitEnemy)
        {
            Debug.Log("Ataque do Samurai atingiu alvos!");
        }
    }

    // Desenha o raio de ataque no editor para facilitar a visualização e o balanceamento.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}