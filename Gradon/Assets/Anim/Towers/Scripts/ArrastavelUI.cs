using UnityEngine;
using UnityEngine.EventSystems; // Essencial para interfaces de arrastar e soltar

public class ArrastavelUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Configuração da Torre")]
    [Tooltip("O Prefab da torre que será criado no mundo do jogo.")]
    public GameObject prefabDaTorre;

    [Header("Feedback Visual")]
    [Tooltip("Cor para quando a posição for válida.")]
    public Color corValida = new Color(0, 1, 0, 0.5f); // Verde semitransparente

    [Tooltip("Cor para quando a posição for inválida.")]
    public Color corInvalida = new Color(1, 0, 0, 0.5f); // Vermelho semitransparente

    // Variáveis privadas
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
        // Verifica se já não estamos arrastando algo e se o prefab foi definido
        if (objetoFantasma == null && prefabDaTorre != null)
        {
            // Instancia o "fantasma" da torre na posição do mouse
            objetoFantasma = Instantiate(prefabDaTorre, GetMouseWorldPosition(), Quaternion.identity);

            // Pega o SpriteRenderer para mudar a cor
            fantasmaSpriteRenderer = objetoFantasma.GetComponent<SpriteRenderer>();

            // Remove componentes que não queremos no fantasma (ex: scripts de ataque, colisor)
            // Isso é opcional, mas recomendado para evitar comportamentos inesperados.
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

            // Lógica para verificar se a posição é válida
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
                // Se a posição é válida, cria a torre de verdade
                // Neste exemplo simples, vamos apenas "confirmar" o fantasma.
                // Em um jogo real, você poderia instanciar um novo prefab aqui.
                objetoFantasma.GetComponent<SpriteRenderer>().color = Color.white; // Restaura a cor original

                // Ative os componentes da torre real aqui (scripts, colisores, etc.)
                // Ex: objetoFantasma.AddComponent<TorreAtiradora>();
            }
            else
            {
                // Se a posição for inválida, destrói o fantasma
                Destroy(objetoFantasma);
            }

            // Limpa a referência para que possamos arrastar novamente
            objetoFantasma = null;
            fantasmaSpriteRenderer = null;
        }
    }

    // Converte a posição do mouse na tela para uma posição no mundo 2D
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        // Z é a distância da câmera. Para 2D, pode ser qualquer valor que a câmera enxergue.
        mouseScreenPos.z = mainCamera.nearClipPlane + 10;
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    // LÓGICA DE VALIDAÇÃO: Aqui você define onde as torres podem ser colocadas.
    // Este é um exemplo simples que define uma área retangular.
    private bool VerificarPosicao(Vector3 position)
    {
        // Exemplo: só pode colocar torres na metade inferior da tela (y < 0)
        // e dentro de uma área x de -5 a 5.
        if (position.y < 0 && position.x > -5 && position.x < 5)
        {
            // Adicione outras checagens aqui, como:
            // - Está colidindo com outra torre? (usando Physics2D.OverlapCircle)
            // - É um terreno válido? (usando Raycasting contra um Tilemap)
            return true;
        }

        return false;
    }
}