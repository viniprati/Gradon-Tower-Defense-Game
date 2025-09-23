// ExplosiveEnemy.cs
using UnityEngine;

public class ExplosiveEnemy : Enemy
{
    [Header("Atributos de Explosão")]
    [SerializeField] private float explosionRadius = 3f;
    // Ele usará a variável 'attackDamage' da classe base para o dano da explosão.
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private LayerMask towerLayer;

    // Sobrescreve o método Die para adicionar a lógica de explosão
    protected override void Die()
    {
        // Cria o efeito visual da explosão
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
                // Usa o 'attackDamage' herdado para causar dano em área
                tower.TakeDamage(this.attackDamage);
            }
        }

        // Chama o método Die() da classe base para dar mana e ser destruído
        base.Die();
    }
}