// ExplosiveEnemy.cs
using UnityEngine;

public class ExplosiveEnemy : Enemy
{
    [Header("Atributos de Explosão")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDamage = 50;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private LayerMask towerLayer; // Configure no Inspector para a layer "Towers"

    /// <summary>
    /// Sobrescreve o método Die para adicionar a lógica de explosão ANTES de morrer.
    /// </summary>
    protected override void Die()
    {
        // Cria o efeito visual da explosão
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Encontra todas as torres dentro do raio de explosão
        Collider2D[] hitTowers = Physics2D.OverlapCircleAll(transform.position, explosionRadius, towerLayer);

        foreach (Collider2D towerCol in hitTowers)
        {
            // Tenta causar dano em cada torre encontrada
            IDamageable tower = towerCol.GetComponent<IDamageable>();
            if (tower != null)
            {
                tower.TakeDamage(explosionDamage);
            }
        }

        // Chama o método Die() da classe base para dar mana e ser destruído
        base.Die();
    }
}