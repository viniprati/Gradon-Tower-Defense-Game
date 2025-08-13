using UnityEngine;
using UnityEngine.UI;

public class TotemHealth : MonoBehaviour
{
    [Header("Configurações da Base")]
    [Tooltip("A vida máxima do totem.")]
    [SerializeField] private float maxHealth = 1000f;

    [Tooltip("Referência opcional para uma barra de vida na UI.")]
    [SerializeField] private Slider healthBar;

    public float CurrentHealth { get; private set; }

    private bool isDestroyed = false;

    void Start()
    {
        CurrentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        CurrentHealth -= damage;

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

        UpdateHealthBar();
        Debug.Log($"Base sofreu {damage} de dano! Vida restante: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = CurrentHealth;
        }
    }

    // --- MÉTODO Die ---
    private void Die()
    {
        isDestroyed = true;

        Debug.Log("<color=red>GAME OVER! A base foi destruída.</color>");


        gameObject.SetActive(false);
    }
}