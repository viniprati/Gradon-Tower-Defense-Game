// TowerInfoPanel.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerInfoPanel : MonoBehaviour
{
    public static TowerInfoPanel instance;

    [Header("Referências da UI")]
    public TextMeshProUGui towerNameText;
    public TextMeshProUGui towerStatsText;
    public Button upgradeButton;
    public TextMeshProUGUI upgradeButtonText;

    private TowerBase currentSelectedTower;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Garante que o painel começa escondido
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Mostra e preenche o painel com as informações de uma torre.
    /// </summary>
    public void ShowPanel(TowerBase tower)
    {
        currentSelectedTower = tower;
        gameObject.SetActive(true);
        UpdatePanel();

        // Configura o botão de upgrade para chamar o método da torre selecionada
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => {
            currentSelectedTower.TryUpgrade();
            UpdatePanel(); // Atualiza o painel após a tentativa de upgrade
        });
    }

    /// <summary>
    /// Esconde o painel.
    /// </summary>
    public void HidePanel()
    {
        currentSelectedTower = null;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Atualiza as informações exibidas no painel.
    /// </summary>
    public void UpdatePanel()
    {
        if (currentSelectedTower == null) return;

        // Supondo que TowerBase tenha essas propriedades
        towerNameText.text = currentSelectedTower.towerName;
        towerStatsText.text = $"Dano: {currentSelectedTower.baseDamage}\nAlcance: {currentSelectedTower.attackRange}\nCadência: {currentSelectedTower.attackRate}/s";

        if (currentSelectedTower.IsAtMaxLevel())
        {
            upgradeButton.interactable = false;
            upgradeButtonText.text = "NÍVEL MÁXIMO";
        }
        else
        {
            int cost = currentSelectedTower.GetNextUpgradeCost();
            // Verifica se o jogador pode pagar pelo upgrade
            upgradeButton.interactable = Totem.instance.currentMana >= cost;
            upgradeButtonText.text = $"UPGRADE ({cost} Mana)";
        }
    }
}