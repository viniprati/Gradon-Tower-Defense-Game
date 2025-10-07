// LevelButton.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
// using UnityEngine.SceneManagement; // Esta linha n�o � mais necess�ria aqui

public class LevelButton : MonoBehaviour
{
    [Header("Refer�ncias de Componentes")]
    [Tooltip("Arraste o componente Text (TMP) que mostrar� o nome da fase aqui.")]
    [SerializeField] private TMP_Text levelNameText;

    [Tooltip("Arraste o componente Button deste pr�prio objeto aqui.")]
    [SerializeField] private Button buttonComponent;

    private LevelData _levelData; // Guarda os dados da fase que este bot�o representa

    /// <summary>
    /// M�todo p�blico chamado pelo LevelSelectController para inicializar este bot�o.
    /// </summary>
    public void Setup(LevelData levelDataToSetup)
    {
        _levelData = levelDataToSetup;

        // Usa 'levelName' que � a vari�vel correta no seu LevelData
        levelNameText.text = _levelData.levelName;

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(OnButtonClick);
    }

    /// <summary>
    /// Este m�todo � executado quando o jogador clica no bot�o.
    /// </summary>
    private void OnButtonClick()
    {
        // --- CORRE��O PRINCIPAL AQUI ---
        // Agora, chamamos o m�todo �nico e correto do GameManager,
        // passando os dados completos da fase que este bot�o representa.

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadLevel(_levelData);
        }
        else
        {
            Debug.LogError("GameManager n�o encontrado! N�o foi poss�vel carregar a fase.");
        }
    }
}