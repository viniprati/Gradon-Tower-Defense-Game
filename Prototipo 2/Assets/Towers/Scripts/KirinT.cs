// ---- CÓDIGO CORRETO PARA MeleeTower.cs ----

using UnityEngine;

public class MeleeTower : MonoBehaviour
{ // A "caixa" da classe começa aqui

    // --- Atributos Originais da Torre ---
    [Header("Atributos da Torre")]
    public float attackRange = 2f;
    public float attackRate = 1f; // Ataques por segundo
    public int damage = 10;

    private float attackCooldown = 0f;
    // ------------------------------------


    // --- Variáveis para o Sistema de Buff ---
    private float originalAttackRate;
    private float originalAttackRange;
    private int originalDamage;
    private bool isBuffed = false;
    // ----------------------------------------


    // O MÉTODO START - Inicializa TUDO
    void Start()
    {
        // Guardamos os valores originais para poder restaurá-los depois
        originalAttackRate = attackRate;
        originalAttackRange = attackRange;
        originalDamage = damage;
    }

    // O MÉTODO UPDATE - Lógica principal da torre
    void Update()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            Attack();
            attackCooldown = 1f / attackRate;
        }
    }

    void Attack()
    {
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var col in collidersInRange)
        {
            if (col.CompareTag("Enemy"))
            {
                Debug.Log("Torre Melee atingiu " + col.name + " com dano " + damage);
                // Ex: col.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
        }
    }


    // --- Métodos para RECEBER e REMOVER o Buff ---
    public void ApplyBuff(float rateMultiplier, float rangeMultiplier, float damageMultiplier)
    {
        if (isBuffed) return;

        isBuffed = true;
        attackRate *= rateMultiplier;
        attackRange *= rangeMultiplier;
        damage = Mathf.RoundToInt(damage * damageMultiplier);
        Debug.Log(gameObject.name + " BUFF APLICADO!");
    }

    public void RemoveBuff()
    {
        if (!isBuffed) return;

        isBuffed = false;
        attackRate = originalAttackRate;
        attackRange = originalAttackRange;
        damage = originalDamage;
        Debug.Log(gameObject.name + " BUFF REMOVIDO!");
    }
    // ------------------------------------------------


    // Apenas para ver o raio de ataque no Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

} // A "caixa" da classe termina aqui. TODO o código deve estar antes desta chave.