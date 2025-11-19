using UnityEngine;

public class ConditionalTowerHealth : MonoBehaviour
{
    [Header("Configuração de Vida")]
    [Tooltip("A vida máxima desta torre.")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;
    private bool isDestroyed = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Esta é a função que os inimigos (o Boss) vão chamar.
    public void TakeDamage(int damageAmount)
    {
        // --- AQUI ESTÁ A LÓGICA PRINCIPAL ---
        // Se o "interruptor" do Boss não estiver ativo, a função para aqui. A torre é invulnerável.
        if (!BossController.isBossActive)
        {
            // Opcional: Adicionar um log para saber que o ataque foi bloqueado.
            // Debug.Log($"Ataque em '{gameObject.name}' bloqueado. O Boss não está em campo.");
            return;
        }
        // ------------------------------------

        // Se o Boss estiver ativo, a lógica de dano continua normalmente.
        if (isDestroyed) return;

        currentHealth -= damageAmount;
        Debug.Log($"'{gameObject.name}' tomou {damageAmount} de dano! Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDestroyed = true;
        // Adicione aqui efeitos de explosão se desejar.
        Destroy(gameObject);
    }
}