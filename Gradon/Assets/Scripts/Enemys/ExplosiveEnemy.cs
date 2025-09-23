// ExplosiveEnemy.cs
using UnityEngine;

public class ExplosiveEnemy : Enemy
{
    [Header("Atributos de Explos�o")]
    [SerializeField] private float explosionRadius = 3f;
    // Ele usar� a vari�vel 'attackDamage' da classe base para o dano da explos�o.
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private LayerMask towerLayer;

    // Sobrescreve o m�todo Die para adicionar a l�gica de explos�o
    protected override void Die()
    {
        // Cria o efeito visual da explos�o
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Encontra todas as torres dentro do raio
        Collider2D[] hitTowers = Physics2D.OverlapCircleAll(transform.position, explosionRadius, towerLayer);
        foreach (Collider2D towerCol in hitTowers)
        {
            Enemy tower = towerCol.GetComponent<Enemy>();
            if (tower != null)
            {
                // Usa o 'attackDamage' herdado para causar dano em �rea
                tower.TakeDamage(this.attackDamage);
            }
        }

        // Chama o m�todo Die() da classe base para dar mana e ser destru�do
        base.Die();
    }
}