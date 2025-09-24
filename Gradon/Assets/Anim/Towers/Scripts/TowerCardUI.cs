using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerCardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Configura��o da Torre")]
    [SerializeField] private GameObject towerPrefab;

    // <-- MELHORIA: O custo agora � pego diretamente do prefab da torre.
    // [SerializeField] private int towerCost = 50; 

    [Header("Refer�ncias da UI")]
    [SerializeField] private Image cooldownImage;

    private GameObject ghostTowerInstance;
    private bool canPlaceTower = false;
    private SpriteRenderer ghostSpriteRenderer; // <-- NOVA VARI�VEL: Para mudar a cor da torre fantasma

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (towerPrefab == null)
        {
            Debug.LogWarning("TowerCardUI: towerPrefab n�o est� atribu�do! Verifique o Inspector.", this.gameObject);
            return;
        }

        // <-- MELHORIA: Pega o custo dinamicamente do prefab da torre para evitar erros.
        int towerCost = towerPrefab.GetComponent<TowerBase>().cost;

        if (Totem.instance != null && Totem.instance.currentMana >= towerCost)
        {
            ghostTowerInstance = Instantiate(towerPrefab);

            var collider = ghostTowerInstance.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;

            var towerScript = ghostTowerInstance.GetComponent<TowerBase>();
            if (towerScript != null) towerScript.enabled = false;

            // <-- NOVA L�GICA: Guarda a refer�ncia do SpriteRenderer
            ghostSpriteRenderer = ghostTowerInstance.GetComponentInChildren<SpriteRenderer>();

            UpdateGhostTowerPosition(eventData);
        }
        else
        {
            Debug.Log("Mana insuficiente!");
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
            // <-- MELHORIA: Usa o custo din�mico
            int towerCost = towerPrefab.GetComponent<TowerBase>().cost;
            Totem.instance.SpendMana(towerCost);

            // Reativa os componentes da torre rec�m-constru�da
            var collider = ghostTowerInstance.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = true;

            var towerScript = ghostTowerInstance.GetComponent<TowerBase>();
            if (towerScript != null) towerScript.enabled = true;

            if (ghostSpriteRenderer != null) ghostSpriteRenderer.color = Color.white;
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

        // ======================================================================
        // NOVA FUN��O DE VALIDA��O ADICIONADA AQUI
        // ======================================================================
        ValidatePlacementPosition();
    }

    private void ValidatePlacementPosition()
    {
        Vector2 towerPosition = ghostTowerInstance.transform.position;
        float totemRadius = Totem.instance.zonaProibidaRaio;
        Vector2 totemPosition = Totem.instance.transform.position;

        // 1. Verifica a dist�ncia do Totem
        bool isTooCloseToTotem = Vector2.Distance(towerPosition, totemPosition) < totemRadius;

        // 2. Verifica se est� em cima de outra torre (usando um OverlapCircle)
        // A LayerMask garante que s� vamos checar por colis�es na layer "Towers"
        LayerMask towerLayer = LayerMask.GetMask("Towers");
        Collider2D towerOverlap = Physics2D.OverlapCircle(towerPosition, 0.5f, towerLayer);

        // Se a posi��o for inv�lida (perto do totem OU em cima de outra torre)
        if (isTooCloseToTotem || towerOverlap != null)
        {
            canPlaceTower = false;
            if (ghostSpriteRenderer != null) ghostSpriteRenderer.color = new Color(1f, 0.5f, 0.5f, 0.7f); // Vermelho transl�cido
        }
        else // Posi��o v�lida
        {
            canPlaceTower = true;
            if (ghostSpriteRenderer != null) ghostSpriteRenderer.color = new Color(0.5f, 1f, 0.5f, 0.7f); // Verde transl�cido
        }
    }
}