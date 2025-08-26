using UnityEngine;
using UnityEngine.EventSystems; // Essencial para interfaces de arrastar e soltar

public class ArrastavelUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Configura��o da Torre")]
    [Tooltip("O Prefab da torre que ser� criado no mundo do jogo.")]
    public GameObject prefabDaTorre;

    [Header("Feedback Visual")]
    [Tooltip("Cor para quando a posi��o for v�lida.")]
    public Color corValida = new Color(0, 1, 0, 0.5f); // Verde semitransparente

    [Tooltip("Cor para quando a posi��o for inv�lida.")]
    public Color corInvalida = new Color(1, 0, 0, 0.5f); // Vermelho semitransparente

    // Vari�veis privadas
    private GameObject objetoFantasma;
    private SpriteRenderer fantasmaSpriteRenderer;
    private bool posicaoValida = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Chamado quando o jogador pressiona o mouse sobre este elemento da UI
    public void OnPointerDown(PointerEventData eventData)
    {
        // Verifica se j� n�o estamos arrastando algo e se o prefab foi definido
        if (objetoFantasma == null && prefabDaTorre != null)
        {
            // Instancia o "fantasma" da torre na posi��o do mouse
            objetoFantasma = Instantiate(prefabDaTorre, GetMouseWorldPosition(), Quaternion.identity);

            // Pega o SpriteRenderer para mudar a cor
            fantasmaSpriteRenderer = objetoFantasma.GetComponent<SpriteRenderer>();

            // Remove componentes que n�o queremos no fantasma (ex: scripts de ataque, colisor)
            // Isso � opcional, mas recomendado para evitar comportamentos inesperados.
            // Ex: Destroy(objetoFantasma.GetComponent<BoxCollider2D>());
        }
    }

    // Chamado continuamente enquanto o jogador arrasta o mouse
    public void OnDrag(PointerEventData eventData)
    {
        if (objetoFantasma != null)
        {
            Vector3 mousePos = GetMouseWorldPosition();
            objetoFantasma.transform.position = mousePos;

            // L�gica para verificar se a posi��o � v�lida
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

    // Chamado quando o jogador solta o mouse
    public void OnPointerUp(PointerEventData eventData)
    {
        if (objetoFantasma != null)
        {
            if (posicaoValida)
            {
                // Se a posi��o � v�lida, cria a torre de verdade
                // Neste exemplo simples, vamos apenas "confirmar" o fantasma.
                // Em um jogo real, voc� poderia instanciar um novo prefab aqui.
                objetoFantasma.GetComponent<SpriteRenderer>().color = Color.white; // Restaura a cor original

                // Ative os componentes da torre real aqui (scripts, colisores, etc.)
                // Ex: objetoFantasma.AddComponent<TorreAtiradora>();
            }
            else
            {
                // Se a posi��o for inv�lida, destr�i o fantasma
                Destroy(objetoFantasma);
            }

            // Limpa a refer�ncia para que possamos arrastar novamente
            objetoFantasma = null;
            fantasmaSpriteRenderer = null;
        }
    }

    // Converte a posi��o do mouse na tela para uma posi��o no mundo 2D
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        // Z � a dist�ncia da c�mera. Para 2D, pode ser qualquer valor que a c�mera enxergue.
        mouseScreenPos.z = mainCamera.nearClipPlane + 10;
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    // L�GICA DE VALIDA��O: Aqui voc� define onde as torres podem ser colocadas.
    // Este � um exemplo simples que define uma �rea retangular.
    private bool VerificarPosicao(Vector3 position)
    {
        // Exemplo: s� pode colocar torres na metade inferior da tela (y < 0)
        // e dentro de uma �rea x de -5 a 5.
        if (position.y < 0 && position.x > -5 && position.x < 5)
        {
            // Adicione outras checagens aqui, como:
            // - Est� colidindo com outra torre? (usando Physics2D.OverlapCircle)
            // - � um terreno v�lido? (usando Raycasting contra um Tilemap)
            return true;
        }

        return false;
    }
}