using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerCardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Configura��o da Torre")]
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private int towerCost = 50;

    [Header("Refer�ncias da UI")]
    [SerializeField] private Image cooldownImage;

    private GameObject ghostTowerInstance;
    private bool canPlaceTower = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (towerPrefab == null)
        {
            Debug.LogWarning("TowerCardUI: towerPrefab n�o est� atribu�do!");
            return;
        }

        if (Totem.instance != null && Totem.instance.currentMana >= towerCost)
        {
            ghostTowerInstance = Instantiate(towerPrefab);

            var collider = ghostTowerInstance.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;

            var towerScript = ghostTowerInstance.GetComponent<TowerBase>();
            if (towerScript != null) towerScript.enabled = false;

            var spriteRenderer = ghostTowerInstance.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);

            UpdateGhostTowerPosition(eventData);
        }
        else
        {
            ghostTowerInstance = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostTowerInstance != null)
        {
            UpdateGhostTowerPosition(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ghostTowerInstance == null) return;

        if (canPlaceTower)
        {
            Totem.instance.SpendMana(towerCost);

            var collider = ghostTowerInstance.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = true;

            var towerScript = ghostTowerInstance.GetComponent<TowerBase>();
            if (towerScript != null) towerScript.enabled = true;

            var spriteRenderer = ghostTowerInstance.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
        }
        else
        {
            Destroy(ghostTowerInstance);
        }

        ghostTowerInstance = null;
    }

    private void UpdateGhostTowerPosition(PointerEventData eventData)
    {
        if (ghostTowerInstance == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        ghostTowerInstance.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);

        // Aqui voc� pode adicionar l�gica de "canPlaceTower" dependendo da zona proibida do Totem
    }
}
