using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public float speed = 2f;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    private float attackTimer;

    private bool isStunned = false;
    private float stunTimer = 0f;

    public Transform playerBase; // ponto que o inimigo quer atacar (ex: centro do telhado)

    void Start()
    {
        currentHealth = maxHealth;
        attackTimer = 0f;
    }

    void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
            }
            return; // para tudo enquanto stun
        }

        if (playerBase == null) return;

        float distance = Vector2.Distance(transform.position, playerBase.position);

        if (distance > attackRange)
        {
            // Move em direção ao playerBase
            Vector2 dir = (playerBase.position - transform.position).normalized;
            transform.position += (Vector3)(dir * speed * Time.deltaTime);
        }
        else
        {
            // Está perto o suficiente para atacar
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }
    }

    void Attack()
    {
        // Aqui você pode chamar um método no playerBase ou sistema de saúde do player
        Debug.Log("Enemy atacou o player!");

    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
