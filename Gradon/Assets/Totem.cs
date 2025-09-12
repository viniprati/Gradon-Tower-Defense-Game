using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Totem : MonoBehaviour, IDamageable
{
    public static Totem instance;

    [Header("Atributos da Base")]
    [SerializeField] private float maxHealth = 1000f;
    public float currentHealth { get; private set; }

    [Header("Configura��o da Zona de Constru��o")]
    [Tooltip("O raio ao redor do Totem onde N�O � permitido construir.")]
    public float zonaProibidaRaio = 3f;

    [Header("Recursos do Jogador")]
    [SerializeField] public float maxMana = 100f;
    public float currentMana { get; private set; }

    [Header("Refer�ncias de Constru��o")]
    [Tooltip("Arraste os PREFABS de todas as torres que o jogador pode construir.")]
    public List<GameObject> availableTowers;

    [Header("Refer�ncias da UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider manaBar;

    private bool isDestroyed = false;
    public bool IsDestroyed => isDestroyed;

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

    public void AddMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + (float)amount, maxMana);
        UpdateManaBar();
    }

    public bool SpendMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            UpdateManaBar();
            return true;
        }
        return false;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.2f, 0, 0.25f);
        Gizmos.DrawWireSphere(transform.position, zonaProibidaRaio);
    }

    #endregion
}