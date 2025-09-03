// ArrastavelUI.cs (Vers�o Definitiva - Torre n�o ataca ao arrastar)

using UnityEngine;
using UnityEngine.EventSystems;

public class ArrastavelUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Configura��o da Torre")]
    [Tooltip("O Prefab da torre que ser� criado no mundo do jogo.")]
    public GameObject prefabDaTorre;

    [Header("Configura��o da Zona de Constru��o")]
    [Tooltip("Arraste o objeto do seu Totem aqui. Se deixado em branco, ser� procurado automaticamente.")]
    public Totem totem;

    [Header("Feedback Visual")]
    public Color corValida = new Color(0, 1, 0, 0.5f);
    public Color corInvalida = new Color(1, 0, 0, 0.5f);
    [Tooltip("Cor usada quando a posi��o � v�lida, mas n�o h� mana suficiente.")]
    public Color corSemMana = new Color(0.5f, 0.5f, 1, 0.5f);

    // Vari�veis internas
    private GameObject objetoFantasma;
    private SpriteRenderer fantasmaSpriteRenderer;
    private bool posicaoValida = false;
    private bool temManaSuficiente = false;
    private Camera mainCamera;
    private TowerBase infoDaTorre; // Usado para ler o custo do prefab

    void Start()
    {
        mainCamera = Camera.main;
        if (totem == null) { totem = FindFirstObjectByType<Totem>(); }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (objetoFantasma == null && prefabDaTorre != null)
        {
            // Pega as informa��es do prefab para checar o custo
            infoDaTorre = prefabDaTorre.GetComponent<TowerBase>();
            if (infoDaTorre == null)
            {
                Debug.LogError($"O prefab '{prefabDaTorre.name}' na carta '{gameObject.name}' n�o tem um componente que herde de 'TowerBase'!", this.gameObject);
                return;
            }

            // 1. Cria o clone (o fantasma)
            objetoFantasma = Instantiate(prefabDaTorre, GetMouseWorldPosition(), Quaternion.identity);
            fantasmaSpriteRenderer = objetoFantasma.GetComponent<SpriteRenderer>();

            // 2. Desativa o comportamento de ataque do CLONE
            TowerBase torreFantasmaScript = objetoFantasma.GetComponent<TowerBase>();
            if (torreFantasmaScript != null)
            {
                torreFantasmaScript.enabled = false;
            }

            // 3. Desativa o colisor do CLONE para n�o interferir na verifica��o de posi��o
            Collider2D fantasmaCollider = objetoFantasma.GetComponent<Collider2D>();
            if (fantasmaCollider != null) fantasmaCollider.enabled = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (objetoFantasma != null)
        {
            Vector3 mousePos = GetMouseWorldPosition();
            objetoFantasma.transform.position = mousePos;

            posicaoValida = VerificarPosicao(mousePos);
            temManaSuficiente = totem.currentMana >= infoDaTorre.cost;

            if (posicaoValida)
            {
                fantasmaSpriteRenderer.color = temManaSuficiente ? corValida : corSemMana;
            }
            else
            {
                fantasmaSpriteRenderer.color = corInvalida;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (objetoFantasma != null)
        {
            if (posicaoValida && temManaSuficiente)
            {
                // Gasta a mana do jogador
                totem.SpendMana(infoDaTorre.cost);

                // 1. Reativa o comportamento de ataque do CLONE, "ligando" a torre
                TowerBase torreFantasmaScript = objetoFantasma.GetComponent<TowerBase>();
                if (torreFantasmaScript != null)
                {
                    torreFantasmaScript.enabled = true;
                }

                // 2. Reativa o colisor do CLONE
                Collider2D fantasmaCollider = objetoFantasma.GetComponent<Collider2D>();
                if (fantasmaCollider != null) fantasmaCollider.enabled = true;

                // 3. Finaliza a constru��o visualmente
                objetoFantasma.GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                // Se a posi��o for inv�lida OU n�o tiver mana, destr�i o fantasma
                Destroy(objetoFantasma);
            }

            // Limpa as vari�veis para a pr�xima constru��o
            objetoFantasma = null;
            fantasmaSpriteRenderer = null;
            infoDaTorre = null;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.nearClipPlane + 10;
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    private bool VerificarPosicao(Vector3 position)
    {
        if (totem != null)
        {
            if (Vector2.Distance(position, totem.transform.position) < totem.zonaProibidaRaio)
            {
                return false;
            }
        }

        if (Physics2D.OverlapCircle(position, 0.5f, LayerMask.GetMask("Towers")) != null)
        {
            return false;
        }
        return true;
    }
}