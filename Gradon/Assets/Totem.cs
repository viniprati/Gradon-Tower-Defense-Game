// Totem.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Para a lista de torres

public class Totem : MonoBehaviour, IDamageable
{
    // --- Singleton para acesso global ---
    public static Totem instance;

    [Header("Atributos da Base")]
    [SerializeField] private float maxHealth = 1000f;

    [Header("Recursos do Jogador")]
    [SerializeField] private float maxMana = 100f;
    public float currentMana { get; private set; }

    [Header("Referências de Construção")]
    [Tooltip("Arraste os PREFABS de todas as torres que o jogador pode construir.")]
    public List<GameObject> availableTowers;

    [Header("Referências da UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider manaBar;

    // --- Variáveis Internas ---
    public float currentHealth { get; private set; }
    private bool isDestroyed = false;

    #region Ciclo de Vida Unity

    void Awake()
    {
        // Configuração do Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;

        // Inicializa as barras de UI
        UpdateHealthBar();
        UpdateManaBar();
    }

    #endregion

    #region Lógica de Vida e Morte

    // Método da interface IDamageable, chamado pelos inimigos.
    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        Debug.Log("<color=red>GAME OVER! A base foi destruída.</color>");

        // Adicione aqui a lógica de fim de jogo (chamar uma tela de derrota, etc.)

        gameObject.SetActive(false);
    }

    #endregion

    #region Lógica de Mana

    public void AddMana(float amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        UpdateManaBar();
    }

    public void SpendMana(float amount)
    {
        currentMana = Mathf.Max(currentMana - amount, 0);
        UpdateManaBar();
    }

    #endregion

    #region Lógica de UI

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    private void UpdateManaBar()
    {
        if (manaBar != null)
        {
            manaBar.maxValue = maxMana;
            manaBar.value = currentMana;
        }
    }

    #endregion
}