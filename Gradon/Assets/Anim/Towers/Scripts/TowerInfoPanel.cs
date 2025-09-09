// TowerInfoPanel.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerInfoPanel : MonoBehaviour
{
    public static TowerInfoPanel instance;

    public TextMeshProUGUI towerNameText;
    public TextMeshProUGUI towerStatsText;
    public Button upgradeButton;
    public TextMeshProUGUI upgradeButtonText;

    private TowerWithBuffs selectedTower; // Agora a referência é para a torre com upgrades

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        gameObject.SetActive(false);
    }

    public void ShowPanel(TowerWithBuffs tower)
    {
        // Desseleciona a torre antiga
        if (selectedTower != null) selectedTower.ShowRangeIndicator(false);

        selectedTower = tower;
        selectedTower.ShowRangeIndicator(true); // Mostra o alcance da nova torre

        gameObject.SetActive(true);
        UpdatePanel();

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => {
            selectedTower.TryUpgrade();
            UpdatePanel();
        });
    }

    public void HidePanel()
    {
        if (selectedTower != null) selectedTower.ShowRangeIndicator(false);
        selectedTower = null;
        gameObject.SetActive(false);
    }

    public void UpdatePanel()
    {
        if (selectedTower == null) return;

        towerNameText.text = selectedTower.towerName;
        towerStatsText.text = $"Dano: {selectedTower.baseDamage}\nAlcance: {selectedTower.attackRange}\nCadência: {selectedTower.attackRate}/s";

        if (selectedTower.IsAtMaxLevel())
        {
            upgradeButton.interactable = false;
            upgradeButtonText.text = "NÍVEL MÁXIMO";
        }
        else
        {
            int cost = selectedTower.GetNextUpgradeCost();
            upgradeButton.interactable = Totem.instance.currentMana >= cost;
            upgradeButtonText.text = $"UPGRADE ({cost} Mana)";
        }
    }
}