// UIManager.cs (Versão Corrigida e Integrada)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // --- SEÇÃO DE MANA ---
    [Header("Referências da UI de Mana")]
    [SerializeField] private Slider manaBar;
    [SerializeField] private TextMeshProUGUI manaText;

    // --- SEÇÃO DE SCORE ---
    [Header("Referências da UI de Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    // --- SEÇÃO DE ONDAS ---
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

    // --- MUDANÇA 1: ADICIONANDO OS "OUVIDOS" (EVENT LISTENERS) ---
    // OnEnable é chamado quando o UIManager é ativado. É o lugar perfeito para começar a ouvir.
    private void OnEnable()
    {
        // Diz ao GameManager: "Quando o evento OnManaChanged acontecer, chame o meu método HandleManaChanged".
        GameManager.OnManaChanged += HandleManaChanged;
        Debug.Log("<color=lime>UIManager está ouvindo as mudanças de mana.</color>");
    }

    // OnDisable é chamado quando o UIManager é desativado. É importante parar de ouvir para evitar erros.
    private void OnDisable()
    {
        GameManager.OnManaChanged -= HandleManaChanged;
    }
    // -------------------------------------------------------------------

    private void Start()
    {
        // A lógica de inicialização de UI continua útil para os outros elementos.
        if (scoreText != null) UpdateScoreUI(0);
        if (waveText != null) waveText.text = "Prepare-se...";

        // A linha abaixo foi removida porque agora o GameManager é o responsável
        // por enviar o valor inicial da mana através do evento OnManaChanged.
        // if (Totem.instance != null) { UpdateManaUI(Totem.instance.currentMana, Totem.instance.maxMana); }
    }

    // --- MUDANÇA 2: O MÉTODO QUE REAGE AO EVENTO DO GAMEMANAGER ---
    /// <summary>
    /// Este método é chamado AUTOMATICAMENTE pelo evento do GameManager sempre que a mana muda.
    /// </summary>
    private void HandleManaChanged(float newManaValue)
    {
        // Adicionamos um log para confirmar que o UIManager recebeu a notificação.
        Debug.Log("<color=yellow>UIManager recebeu o evento OnManaChanged com o valor: " + newManaValue + "</color>");

        if (manaText != null)
        {
            // Atualiza o texto visual. Usaremos o formato simples que você tem na sua imagem.
            manaText.text = "Mana: " + Mathf.RoundToInt(newManaValue);
        }
        else
        {
            Debug.LogError("ERRO no UIManager: A referência para o 'Mana Text' não foi definida no Inspector!");
        }
        if (manaBar != null && Totem.instance != null)
        {
             manaBar.value = newManaValue / Totem.instance.maxMana;
        }
    }
    // -------------------------------------------------------------------

    // O seu método original de UpdateManaUI foi mantido caso você o use em outro lugar.
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

    // Métodos para Score e Ondas (sem mudanças)
    public void UpdateScoreUI(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + newScore.ToString();
        }
    }

    public void UpdateWaveUI(string message)
    {
        if (waveText != null)
        {
            waveText.text = message;
        }
    }
}