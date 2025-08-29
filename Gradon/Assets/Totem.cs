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

    // --- CORRE��O AQUI ---
    // Adicionamos a vari�vel p�blica para o raio da zona proibida.
    // 'public' � a palavra-chave que permite que outros scripts (como o ArrastavelUI)
    // leiam o valor desta vari�vel.
    [Header("Configura��o da Zona de Constru��o")]
    [Tooltip("O raio ao redor do Totem onde N�O � permitido construir.")]
    public float zonaProibidaRaio = 3f;

    [Header("Recursos do Jogador")]
    [SerializeField] private float maxMana = 100f;
    public float currentMana { get; private set; }

    [Header("Refer�ncias de Constru��o")]
    [Tooltip("Arraste os PREFABS de todas as torres que o jogador pode construir.")]
    public List<GameObject> availableTowers;

    [Header("Refer�ncias da UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider manaBar;

    // --- Vari�veis Internas ---
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

    #region L�gica de Vida e Morte

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
        Debug.Log("<color=red>GAME OVER! A base foi destru�da.</color>");
        gameObject.SetActive(false);
    }

    #endregion

    #region L�gica de Mana

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

    #region L�gica de UI

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
    /// Desenha o c�rculo da zona proibida no editor da Unity.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.2f, 0, 0.25f);
        Gizmos.DrawWireSphere(transform.position, zonaProibidaRaio);
    }

    #endregion
}