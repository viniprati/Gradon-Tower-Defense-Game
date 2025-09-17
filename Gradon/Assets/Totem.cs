// Totem.cs (Com sistema de vida e barra de vida funcional)

using UnityEngine;
using UnityEngine.UI; // Essencial para usar o componente Slider
using System.Collections.Generic;

public class Totem : MonoBehaviour, IDamageable // Garante que o Totem possa receber dano
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
    [Tooltip("Arraste aqui o Slider da barra de vida do Totem.")]
    [SerializeField] private Slider healthBar; // A refer�ncia para a barra de vida

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

        // Pega a mana inicial a partir dos dados da fase
        currentMana = (GameManager.instance != null && GameManager.instance.currentLevelData != null)
            ? GameManager.instance.currentLevelData.initialMana
            : maxMana;

        // Inicializa a barra de vida no come�o do jogo
        UpdateHealthBar();

        // Notifica a UI sobre o estado inicial da mana
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateManaUI(currentMana, maxMana);
        }
    }

    #endregion

    // --- L�GICA DE VIDA E MORTE RESTAURADA ---
    #region L�gica de Vida e Morte

    /// <summary>
    /// M�todo p�blico para que inimigos e outras fontes possam causar dano ao Totem.
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        // Atualiza a barra de vida visualmente
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    // Sobrecarga para aceitar dano do tipo int
    public void TakeDamage(int damage)
    {
        TakeDamage((float)damage);
    }

    private void Die()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        Debug.Log("<color=red>GAME OVER! A base foi destru�da.</color>");

        // Avisa ao GameManager que o jogador perdeu
        if (GameManager.instance != null) GameManager.instance.HandleGameOver(false);

        // Desativa o objeto para que ele n�o possa mais ser alvo
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

    // --- L�GICA DA BARRA DE VIDA RESTAURADA ---
    #region L�gica de UI

    /// <summary>
    /// Atualiza o Slider da barra de vida com base na vida atual e m�xima.
    /// </summary>
    private void UpdateHealthBar()
    {
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