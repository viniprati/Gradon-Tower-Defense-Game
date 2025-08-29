// Totem.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Totem : MonoBehaviour, IDamageable
{
    // --- Singleton para acesso global ---
    public static Totem instance;

    [Header("Atributos da Base")]
    [SerializeField] private float maxHealth = 1000f;

    // --- CORREÇÃO AQUI ---
    // Adicionamos a variável pública para o raio da zona proibida.
    // 'public' é a palavra-chave que permite que outros scripts (como o ArrastavelUI)
    // leiam o valor desta variável.
    [Header("Configuração da Zona de Construção")]
    [Tooltip("O raio ao redor do Totem onde NÃO é permitido construir.")]
    public float zonaProibidaRaio = 3f;

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
        UpdateHealthBar();
        UpdateManaBar();
    }

    #endregion

    #region Lógica de Vida e Morte

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

    #region Editor Visuals

    /// <summary>
    /// Desenha o círculo da zona proibida no editor da Unity.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.2f, 0, 0.25f);
        Gizmos.DrawWireSphere(transform.position, zonaProibidaRaio);
    }

    #endregion
}