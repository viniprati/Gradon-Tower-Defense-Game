// UIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // --- SEÇÃO DE MANA (JÁ EXISTENTE) ---
    [Header("Referências da UI de Mana")]
    [SerializeField] private Slider manaBar;
    [SerializeField] private TextMeshProUGUI manaText;

    // --- NOVA SEÇÃO DE SCORE ---
    [Header("Referências da UI de Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    // --- NOVA SEÇÃO DE ONDAS ---
    [Header("Referências da UI de Ondas")]
    [SerializeField] private TextMeshProUGUI waveText;


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

    /// <summary>
    /// Inicializa a UI com os valores corretos no início da fase.
    /// </summary>
    private void Start()
    {
        // Garante que o score comece zerado na UI
        if (scoreText != null) UpdateScoreUI(0);

        // Garante que a mana comece com o valor inicial do Totem
        if (Totem.instance != null)
        {
            UpdateManaUI(Totem.instance.currentMana, Totem.instance.maxMana);
        }

        // Texto inicial para as ondas
        if (waveText != null) waveText.text = "Prepare-se...";
    }

    // --- SEU MÉTODO DE MANA (SEM MUDANÇAS) ---
    public void UpdateManaUI(float currentMana, float maxMana)
    {
        if (manaBar != null)
        {
            manaBar.value = currentMana / maxMana;
        }
        if (manaText != null)
        {
            manaText.text = $"{currentMana.ToString("F0")} / {maxMana.ToString("F0")}";
        }
    }

    // --- NOVO MÉTODO PARA O SCORE ---
    /// <summary>
    /// Atualiza o texto do score na tela.
    /// </summary>
    public void UpdateScoreUI(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + newScore.ToString();
        }
    }

    // --- NOVO MÉTODO PARA AS ONDAS ---
    /// <summary>
    /// Atualiza o texto de informação das ondas na tela.
    /// </summary>
    public void UpdateWaveUI(string message)
    {
        if (waveText != null)
        {
            waveText.text = message;
        }
    }
}