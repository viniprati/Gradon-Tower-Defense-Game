using UnityEngine;
using UnityEngine.EventSystems;

public class ArrastavelUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Configura��o da Torre")]
    [Tooltip("O Prefab da torre que ser� criado no mundo do jogo.")]
    public GameObject prefabDaTorre;

    [Header("Configura��o da Zona de Constru��o")]
    [Tooltip("Arraste o objeto do seu Totem aqui. Se deixado em branco, ser� procurado automaticamente.")]
    public Totem totem; // Refer�ncia para o SCRIPT do Totem.

    [Header("Feedback Visual")]
    public Color corValida = new Color(0, 1, 0, 0.5f);
    public Color corInvalida = new Color(1, 0, 0, 0.5f);

    // Vari�veis internas para gerenciar o processo de arrastar
    private GameObject objetoFantasma;
    private SpriteRenderer fantasmaSpriteRenderer;
    private bool posicaoValida = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Se o Totem n�o foi arrastado no Inspector, tenta encontr�-lo na cena
        if (totem == null)
        {
            totem = FindFirstObjectByType<Totem>();
            if (totem == null)
            {
                Debug.LogError("O Totem n�o foi encontrado na cena! A checagem da zona de constru��o n�o funcionar�.", this.gameObject);
            }
        }
    }

    /// <summary>
    /// Chamado no exato momento em que o jogador clica nesta imagem da UI.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // S� cria um fantasma se o prefab existir e n�o estivermos arrastando nada
        if (objetoFantasma == null && prefabDaTorre != null)
        {
            objetoFantasma = Instantiate(prefabDaTorre, GetMouseWorldPosition(), Quaternion.identity);
            fantasmaSpriteRenderer = objetoFantasma.GetComponent<SpriteRenderer>();

            // Desativa o colisor do fantasma para que ele n�o interfira na nossa checagem de posi��o
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
            // Atualiza a posi��o do fantasma para seguir o mouse
            Vector3 mousePos = GetMouseWorldPosition();
            objetoFantasma.transform.position = mousePos;

            // Verifica se a posi��o atual � v�lida e atualiza a cor do fantasma
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
            // Se a posi��o final era v�lida...
            if (posicaoValida)
            {
                // ...a torre � "confirmada".
                // Reativa o colisor da torre para que ela possa interagir com o jogo.
                Collider2D fantasmaCollider = objetoFantasma.GetComponent<Collider2D>();
                if (fantasmaCollider != null)
                {
                    fantasmaCollider.enabled = true;
                }

                // Restaura a cor original do sprite
                objetoFantasma.GetComponent<SpriteRenderer>().color = Color.white;

                // Aqui voc� pode adicionar l�gica extra, como ativar os scripts de ataque da torre.
            }
            else
            {
                // Se a posi��o era inv�lida, simplesmente destr�i o fantasma.
                Destroy(objetoFantasma);
            }

            // Limpa as vari�veis para a pr�xima vez que o jogador arrastar
            objetoFantasma = null;
            fantasmaSpriteRenderer = null;
        }
    }

    /// <summary>
    /// Converte a posi��o do mouse na tela para uma posi��o no mundo do jogo.
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.nearClipPlane + 10;
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    /// <summary>
    /// Verifica se uma determinada posi��o � v�lida para construir uma torre.
    /// </summary>
    private bool VerificarPosicao(Vector3 position)
    {
        // Checagem 1: Zona Proibida do Totem
        if (totem != null)
        {
            float distanciaDoTotem = Vector2.Distance(position, totem.transform.position);
            // Se estiver dentro do raio, a posi��o � inv�lida.
            if (distanciaDoTotem < totem.zonaProibidaRaio)
            {
                return false;
            }
        }

        // Checagem 2: Colis�o com Outras Torres
        float checkRadius = 0.5f;
        Collider2D colliderHit = Physics2D.OverlapCircle(position, checkRadius, LayerMask.GetMask("Towers"));
        // Se encontrou um colisor, a posi��o � inv�lida.
        if (colliderHit != null)
        {
            return false;
        }

        // Se passou por todas as checagens, a posi��o � v�lida!
        return true;
    }
}