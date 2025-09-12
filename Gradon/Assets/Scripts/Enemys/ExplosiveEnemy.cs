using UnityEngine;

public class ExplosiveEnemy : Enemy
{
    public float explosionRadius = 2f;
    public int explosionDamage = 50;

    void Update()
    {
        rb.velocity = Vector2.left * speed;
    }

    public override void Attack()
    {
        // Explode ao atacar (ou quando morrer)
        Debug.Log("ExplosiveEnemy explodiu!");
        // Aqui voc� pode adicionar l�gica real de dano na �rea
    }

    protected override void Die()
    {
        Attack(); // explode ao morrer
        base.Die();
    }
}
