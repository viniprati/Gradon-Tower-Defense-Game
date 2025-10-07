// LevelButton.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
// using UnityEngine.SceneManagement; // Esta linha não é mais necessária aqui

public class LevelButton : MonoBehaviour
{
    [Header("Referências de Componentes")]
    [Tooltip("Arraste o componente Text (TMP) que mostrará o nome da fase aqui.")]
    [SerializeField] private TMP_Text levelNameText;

    [Tooltip("Arraste o componente Button deste próprio objeto aqui.")]
    [SerializeField] private Button buttonComponent;

    private LevelData _levelData; // Guarda os dados da fase que este botão representa

    /// <summary>
    /// Método público chamado pelo LevelSelectController para inicializar este botão.
    /// </summary>
    public void Setup(LevelData levelDataToSetup)
    {
        _levelData = levelDataToSetup;

        // Usa 'levelName' que é a variável correta no seu LevelData
        levelNameText.text = _levelData.levelName;

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(OnButtonClick);
    }

    /// <summary>
    /// Este método é executado quando o jogador clica no botão.
    /// </summary>
    private void OnButtonClick()
    {
        // --- CORREÇÃO PRINCIPAL AQUI ---
        // Agora, chamamos o método único e correto do GameManager,
        // passando os dados completos da fase que este botão representa.

        if (GameManager.instance != null)
        {
            GameManager.instance.LoadLevel(_levelData);
        }
        else
        {
            Debug.LogError("GameManager não encontrado! Não foi possível carregar a fase.");
        }
    }
}