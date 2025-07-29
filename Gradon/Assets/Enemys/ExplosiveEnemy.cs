// ExplosiveEnemy.cs
using UnityEngine;

public class ExplosiveEnemy : EnemyBase
{
    [Header("Atributos Explosivos")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private GameObject explosionEffectPrefab; // Efeito visual da explosão

    // Movimento igual ao inimigo normal
    protected override Vector2 HandleMovement()
    {
        // Apenas retorna a direção para o jogador. A classe base cuidará da velocidade.
        if (playerTransform != null)
        {
            return (playerTransform.position - transform.position).normalized;
        }
        return Vector2.zero;
    }

    // Sobrescrevemos o método Die para adicionar a lógica da explosão
    protected override void Die()
    {
        // Antes de chamar a lógica base de morte (que destrói o objeto),
        // nós executamos a explosão.

        // 1. Instancia o efeito visual da explosão
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. Encontra todos os colisores dentro do raio de explosão
        Collider2D[] collidersInExplosion = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        // 3. Itera sobre os objetos encontrados e causa dano
        foreach (Collider2D hitCollider in collidersInExplosion)
        {
            // Tenta pegar o componente EnemyBase do objeto atingido
            EnemyBase otherEnemy = hitCollider.GetComponent<EnemyBase>();

            // Se for um inimigo e não for ele mesmo, causa dano
            if (otherEnemy != null && otherEnemy != this)
            {
                otherEnemy.TakeDamage(explosionDamage);
            }

            // Você também pode adicionar dano ao player aqui se quiser
            // PlayerController player = hitCollider.GetComponent<PlayerController>();
            // if (player != null) { /* ... cause dano no player ... */ }
        }

        // 4. Agora sim, executa a lógica base de morte (dar score, dropar moeda, destruir)
        base.Die();
    }

    // Desenha o raio da explosão no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}