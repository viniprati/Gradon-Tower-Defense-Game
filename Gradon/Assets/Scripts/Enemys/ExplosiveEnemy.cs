using UnityEngine;

public class ExplosiveEnemy : Enemy
{
    [Header("Configurações do Explosive Enemy")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionDamage = 75f;

    protected override void Start()
    {
        base.Start();
    }

    public override void Attack()
    {
        Die();
    }

    protected override void Die()
    {
        if (!IsDead)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (Collider2D hitCollider in colliders)
            {
                if (hitCollider.CompareTag("Totem"))
                {
                    hitCollider.GetComponent<Totem>()?.TakeDamage(explosionDamage);
                }
            }
        }
        base.Die();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}