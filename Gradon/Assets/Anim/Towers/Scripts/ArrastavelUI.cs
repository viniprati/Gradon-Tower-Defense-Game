using UnityEngine;
using UnityEngine.EventSystems;

public class ArrastavelUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Configuração da Torre")]
    [Tooltip("O Prefab da torre que será criado no mundo do jogo.")]
    public GameObject prefabDaTorre;

    [Header("Configuração da Zona de Construção")]
    [Tooltip("Arraste o objeto do seu Totem aqui. Se deixado em branco, será procurado automaticamente.")]
    public Totem totem; // Referência para o SCRIPT do Totem.

    [Header("Feedback Visual")]
    public Color corValida = new Color(0, 1, 0, 0.5f);
    public Color corInvalida = new Color(1, 0, 0, 0.5f);

    // Variáveis internas para gerenciar o processo de arrastar
    private GameObject objetoFantasma;
    private SpriteRenderer fantasmaSpriteRenderer;
    private bool posicaoValida = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Se o Totem não foi arrastado no Inspector, tenta encontrá-lo na cena
        if (totem == null)
        {
            totem = FindFirstObjectByType<Totem>();
            if (totem == null)
            {
                Debug.LogError("O Totem não foi encontrado na cena! A checagem da zona de construção não funcionará.", this.gameObject);
            }
        }
    }

    /// <summary>
    /// Chamado no exato momento em que o jogador clica nesta imagem da UI.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // Só cria um fantasma se o prefab existir e não estivermos arrastando nada
        if (objetoFantasma == null && prefabDaTorre != null)
        {
            objetoFantasma = Instantiate(prefabDaTorre, GetMouseWorldPosition(), Quaternion.identity);
            fantasmaSpriteRenderer = objetoFantasma.GetComponent<SpriteRenderer>();

            // Desativa o colisor do fantasma para que ele não interfira na nossa checagem de posição
            Collider2D fantasmaCollider = objetoFantasma.GetComponent<Collider2D>();
            if (fantasmaCollider != null)
            {
                fantasmaCollider.enabled = false;
            }
        }
    }

    /// <summary>
    /// Chamado continuamente enquanto o jogador segura o clique e move o mouse.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (objetoFantasma != null)
        {
            // Atualiza a posição do fantasma para seguir o mouse
            Vector3 mousePos = GetMouseWorldPosition();
            objetoFantasma.transform.position = mousePos;

            // Verifica se a posição atual é válida e atualiza a cor do fantasma
            if (VerificarPosicao(mousePos))
            {
                posicaoValida = true;
                fantasmaSpriteRenderer.color = corValida;
            }
            else
            {
                posicaoValida = false;
                fantasmaSpriteRenderer.color = corInvalida;
            }
        }
    }

    /// <summary>
    /// Chamado no momento em que o jogador solta o clique do mouse.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (objetoFantasma != null)
        {
            // Se a posição final era válida...
            if (posicaoValida)
            {
                // ...a torre é "confirmada".
                // Reativa o colisor da torre para que ela possa interagir com o jogo.
                Collider2D fantasmaCollider = objetoFantasma.GetComponent<Collider2D>();
                if (fantasmaCollider != null)
                {
                    fantasmaCollider.enabled = true;
                }

                // Restaura a cor original do sprite
                objetoFantasma.GetComponent<SpriteRenderer>().color = Color.white;

                // Aqui você pode adicionar lógica extra, como ativar os scripts de ataque da torre.
            }
            else
            {
                // Se a posição era inválida, simplesmente destrói o fantasma.
                Destroy(objetoFantasma);
            }

            // Limpa as variáveis para a próxima vez que o jogador arrastar
            objetoFantasma = null;
            fantasmaSpriteRenderer = null;
        }
    }

    /// <summary>
    /// Converte a posição do mouse na tela para uma posição no mundo do jogo.
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.nearClipPlane + 10;
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    /// <summary>
    /// Verifica se uma determinada posição é válida para construir uma torre.
    /// </summary>
    private bool VerificarPosicao(Vector3 position)
    {
        // Checagem 1: Zona Proibida do Totem
        if (totem != null)
        {
            float distanciaDoTotem = Vector2.Distance(position, totem.transform.position);
            // Se estiver dentro do raio, a posição é inválida.
            if (distanciaDoTotem < totem.zonaProibidaRaio)
            {
                return false;
            }
        }

        // Checagem 2: Colisão com Outras Torres
        float checkRadius = 0.5f;
        Collider2D colliderHit = Physics2D.OverlapCircle(position, checkRadius, LayerMask.GetMask("Towers"));
        // Se encontrou um colisor, a posição é inválida.
        if (colliderHit != null)
        {
            return false;
        }

        // Se passou por todas as checagens, a posição é válida!
        return true;
    }
}