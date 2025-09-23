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

    [Header("Configura��o da Zona de Constru��o")]
    [Tooltip("O raio ao redor do Totem onde N�O � permitido construir.")]
    public float zonaProibidaRaio = 3f;

    [Header("Recursos do Jogador")]
    [SerializeField] public float maxMana = 100f;
    public float currentMana { get; private set; }

    [Header("Refer�ncias da UI")]
    [Tooltip("Arraste aqui o Slider da barra de vida do Totem (opcional).")]
    [SerializeField] private Slider healthBar;

    // Vari�veis internas
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
        currentMana = (GameManager.instance != null && GameManager.instance.currentLevelData != null)
            ? GameManager.instance.currentLevelData.initialMana
            : maxMana;

        UpdateHealthBar();

        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateManaUI(currentMana, maxMana);
        }
    }

    #endregion

    #region L�gica de Vida e Morte

    /// <summary>
    /// M�todo principal para receber dano, cumprindo o contrato da interface IDamageable.
    /// � chamado por inimigos e outras fontes de dano.
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

        Debug.Log("<color=red>GAME OVER! A base foi destru�da.</color>");

        if (GameManager.instance != null) GameManager.instance.HandleGameOver(false);

        gameObject.SetActive(false);
    }

    #endregion

    #region L�gica de Mana

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

    #region L�gica de UI

    /// <summary>
    /// Atualiza o Slider da barra de vida. Cont�m uma checagem de seguran�a.
    /// </summary>
    private void UpdateHealthBar()
    {
        // Esta checagem de seguran�a (if healthBar n�o � nulo) � a raz�o
        // pela qual o jogo n�o quebra se voc� esquecer de conectar o Slider.
        // A l�gica de dano continua funcionando, apenas a parte visual � pulada.
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