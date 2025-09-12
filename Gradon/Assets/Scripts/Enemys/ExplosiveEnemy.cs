// ExplosiveEnemy.cs (Mantendo o movimento linear para este, mas pode ser adaptado)
using UnityEngine;

public class ExplosiveEnemy : Enemy
{
    public float explosionRadius = 2f;
    public int explosionDamage = 50;

    void Update()
    {
        // Se quiser que ele desacelere perto do alvo, chame MoveTowardsTarget() aqui
        // Mas para manter a ideia original de movimento linear, podemos deixar assim:
        rb.velocity = Vector2.left * speed;
    }

    public override void Attack()
    {
        // Explode ao atacar (ou quando morrer)
        Debug.Log("ExplosiveEnemy explodiu!");
        // Aqui você pode adicionar lógica real de dano na área
    }

    protected override void Die()
    {
        Attack(); // explode ao morrer
        base.Die();
    }
}