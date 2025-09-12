using UnityEngine;

public class ExplosiveEnemy : Enemy
{
    [Header("Atributos de Explosão")]
    [SerializeField] private float explosionRange = 2f;
    [SerializeField] private float explosionDamage = 40f;

    protected override void Update()
    {
        if (isDead || currentTarget == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        if (distanceToTarget > explosionRange)
        {
            base.Update(); // movimento padrão
        }
        else
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRange);
        foreach (var hit in hits)
        {
            IDamageable targetHealth = hit.GetComponent<IDamageable>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(explosionDamage);
            }
        }

        Debug.Log(gameObject.name + " explodiu causando dano em área!");
        Die(); // já morre após explodir
    }
}
