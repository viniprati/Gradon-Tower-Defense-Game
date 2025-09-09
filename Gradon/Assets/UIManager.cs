// UIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // --- SE��O DE MANA (J� EXISTENTE) ---
    [Header("Refer�ncias da UI de Mana")]
    [SerializeField] private Slider manaBar;
    [SerializeField] private TextMeshProUGUI manaText;

    // --- NOVA SE��O DE SCORE ---
    [Header("Refer�ncias da UI de Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    // --- NOVA SE��O DE ONDAS ---
    [Header("Refer�ncias da UI de Ondas")]
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
    /// Inicializa a UI com os valores corretos no in�cio da fase.
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

    // --- SEU M�TODO DE MANA (SEM MUDAN�AS) ---
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

    // --- NOVO M�TODO PARA O SCORE ---
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

    // --- NOVO M�TODO PARA AS ONDAS ---
    /// <summary>
    /// Atualiza o texto de informa��o das ondas na tela.
    /// </summary>
    public void UpdateWaveUI(string message)
    {
        if (waveText != null)
        {
            waveText.text = message;
        }
    }
}