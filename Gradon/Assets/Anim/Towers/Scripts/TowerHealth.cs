using UnityEngine;

public class TowerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 200;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " tomou dano! Vida restante: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " foi destruído!");
        Destroy(gameObject);
    }
}