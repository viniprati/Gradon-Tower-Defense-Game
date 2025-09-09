// TowerInfoPanel.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerInfoPanel : MonoBehaviour
{
    public static TowerInfoPanel instance;

    [Header("Refer�ncias da UI")]
    public TextMeshProUGui towerNameText;
    public TextMeshProUGui towerStatsText;
    public Button upgradeButton;
    public TextMeshProUGUI upgradeButtonText;

    private TowerBase currentSelectedTower;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Garante que o painel come�a escondido
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Mostra e preenche o painel com as informa��es de uma torre.
    /// </summary>
    public void ShowPanel(TowerBase tower)
    {
        currentSelectedTower = tower;
        gameObject.SetActive(true);
        UpdatePanel();

        // Configura o bot�o de upgrade para chamar o m�todo da torre selecionada
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => {
            currentSelectedTower.TryUpgrade();
            UpdatePanel(); // Atualiza o painel ap�s a tentativa de upgrade
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
    /// Atualiza as informa��es exibidas no painel.
    /// </summary>
    public void UpdatePanel()
    {
        if (currentSelectedTower == null) return;

        // Supondo que TowerBase tenha essas propriedades
        towerNameText.text = currentSelectedTower.towerName;
        towerStatsText.text = $"Dano: {currentSelectedTower.baseDamage}\nAlcance: {currentSelectedTower.attackRange}\nCad�ncia: {currentSelectedTower.attackRate}/s";

        if (currentSelectedTower.IsAtMaxLevel())
        {
            upgradeButton.interactable = false;
            upgradeButtonText.text = "N�VEL M�XIMO";
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