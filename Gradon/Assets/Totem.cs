// Totem.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// O Totem "assina o contrato" de que pode receber dano.
public class Totem : MonoBehaviour
{
    public static Totem instance;

    [Header("Atributos da Base")]
    [SerializeField] private float maxHealth = 1000f;
    public float currentHealth { get; private set; }

    [Header("Configuração da Zona de Construção")]
    [Tooltip("O raio ao redor do Totem onde NÃO é permitido construir.")]
    public float zonaProibidaRaio = 3f;

    [Header("Recursos do Jogador")]
    [SerializeField] public float maxMana = 100f;
    public float currentMana { get; private set; }

    [Header("Referências da UI")]
    [Tooltip("Arraste aqui o Slider da barra de vida do Totem (opcional).")]
    [SerializeField] private Slider healthBar;

    // Variáveis internas
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
        currentMana = (GameManager.Instance != null && GameManager.Instance.currentLevelData != null)
            ? GameManager.Instance.currentLevelData.startingMana
            : maxMana;

        UpdateHealthBar();

        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateManaUI(currentMana, maxMana);
        }
    }

    #endregion

    #region Lógica de Vida e Morte

    /// <summary>
    /// Método principal para receber dano, cumprindo o contrato da interface IDamageable.
    /// É chamado por inimigos e outras fontes de dano.
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        // Atualiza a barra de vida visualmente.
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

        if (GameManager.Instance != null) GameManager.Instance.HandleGameOver(false);

        gameObject.SetActive(false);
    }

    #endregion

    #region Lógica de Mana

    public void AddMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        if (UIManager.instance != null) UIManager.instance.UpdateManaUI(currentMana, maxMana);
    }

    public bool SpendMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            if (UIManager.instance != null) UIManager.instance.UpdateManaUI(currentMana, maxMana);
            return true;
        }
        return false;
    }

    #endregion

    #region Lógica de UI

    /// <summary>
    /// Atualiza o Slider da barra de vida. Contém uma checagem de segurança.
    /// </summary>
    private void UpdateHealthBar()
    {
        // Esta checagem de segurança (if healthBar não é nulo) é a razão
        // pela qual o jogo não quebra se você esquecer de conectar o Slider.
        // A lógica de dano continua funcionando, apenas a parte visual é pulada.
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
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