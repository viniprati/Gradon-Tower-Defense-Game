// ExplosiveEnemy.cs
using UnityEngine;

public class ExplosiveEnemy : Enemy
{
    [Header("Atributos de Explos�o")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDamage = 50;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private LayerMask towerLayer; // Configure no Inspector para a layer "Towers"

    /// <summary>
    /// Sobrescreve o m�todo Die para adicionar a l�gica de explos�o ANTES de morrer.
    /// </summary>
    protected override void Die()
    {
        // Cria o efeito visual da explos�o
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Encontra todas as torres dentro do raio de explos�o
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

        // Chama o m�todo Die() da classe base para dar mana e ser destru�do
        base.Die();
    }
}