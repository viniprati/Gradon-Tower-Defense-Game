using UnityEngine;

public class RangedEnemy : Enemy
{
    public GameObject projectile;
    public float fireRate = 1f;
    private float fireCooldown = 0f;

    void Update()
    {
        rb.velocity = Vector2.left * speed;
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Attack();
            fireCooldown = fireRate;
        }
    }

    public override void Attack()
    {
        Instantiate(projectile, transform.position, Quaternion.identity);
        Debug.Log("RangedEnemy atirou um projétil!");
    }
}
