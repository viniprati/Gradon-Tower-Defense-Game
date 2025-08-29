// ArrastavelUI.cs (Atualizado com Lógica de Custo de Mana)

using UnityEngine;
using UnityEngine.EventSystems;

public class ArrastavelUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Configuração da Torre")]
    public GameObject prefabDaTorre;

    [Header("Configuração da Zona de Construção")]
    public Totem totem;

    [Header("Feedback Visual")]
    public Color corValida = new Color(0, 1, 0, 0.5f);
    public Color corInvalida = new Color(1, 0, 0, 0.5f);
    [Tooltip("Cor usada quando a posição é válida, mas não há mana suficiente.")]
    public Color corSemMana = new Color(0.5f, 0.5f, 1, 0.5f); // Azul/Roxo semitransparente

    private GameObject objetoFantasma;
    private SpriteRenderer fantasmaSpriteRenderer;
    private bool posicaoValida = false;
    private bool temManaSuficiente = false; // <-- NOVA VARIÁVEL
    private Camera mainCamera;
    private TowerBase infoDaTorre; // <-- NOVA VARIÁVEL para guardar o custo

    void Start()
    {
        mainCamera = Camera.main;
        if (totem == null) { totem = FindFirstObjectByType<Totem>(); }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (objetoFantasma == null && prefabDaTorre != null)
        {
            // Pega as informações da torre ANTES de criar o fantasma
            infoDaTorre = prefabDaTorre.GetComponent<TowerBase>();
            if (infoDaTorre == null)
            {
                Debug.LogError("O prefab da torre não tem um componente 'TowerBase'!", this.gameObject);
                return;
            }

            // Cria o fantasma
            objetoFantasma = Instantiate(prefabDaTorre, GetMouseWorldPosition(), Quaternion.identity);
            fantasmaSpriteRenderer = objetoFantasma.GetComponent<SpriteRenderer>();

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

            // --- LÓGICA DE FEEDBACK VISUAL ATUALIZADA ---
            posicaoValida = VerificarPosicao(mousePos);
            temManaSuficiente = totem.currentMana >= infoDaTorre.cost;

            if (posicaoValida)
            {
                // Se a posição é válida, a cor depende se temos mana ou não
                fantasmaSpriteRenderer.color = temManaSuficiente ? corValida : corSemMana;
            }
            else
            {
                // Se a posição for inválida, a cor é sempre inválida
                fantasmaSpriteRenderer.color = corInvalida;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (objetoFantasma != null)
        {
            // --- LÓGICA DE CONSTRUÇÃO ATUALIZADA ---
            // A construção só é permitida se a posição for válida E se tivermos mana
            if (posicaoValida && temManaSuficiente)
            {
                // Gasta a mana do jogador
                totem.SpendMana(infoDaTorre.cost);

                // Confirma a construção da torre
                Collider2D fantasmaCollider = objetoFantasma.GetComponent<Collider2D>();
                if (fantasmaCollider != null) fantasmaCollider.enabled = true;
                objetoFantasma.GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                // Se a posição for inválida OU não tiver mana, destrói o fantasma
                Destroy(objetoFantasma);
            }

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