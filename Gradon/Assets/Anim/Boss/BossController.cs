using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Configura��es de Movimento")]
    [Tooltip("A velocidade com que o Boss se move em dire��o ao Totem.")]
    [SerializeField] private float velocidade = 2f;

    // A refer�ncia para o Transform do Totem.
    // Agora � privada, pois o script vai preench�-la sozinho.
    private Transform totemAlvo;

    // A fun��o Start � chamada uma �nica vez quando o Boss � criado.
    // � o lugar perfeito para encontrar o alvo.
    void Start()
    {
        // 1. ENCONTRAR O TOTEM AUTOMATICAMENTE:
        // Procura por qualquer GameObject na cena que tenha a tag "TotemPrincipal".
        GameObject totemObject = GameObject.FindGameObjectWithTag("TotemPrincipal");

        // 2. VERIFICAR SE O TOTEM FOI ENCONTRADO:
        if (totemObject != null)
        {
            // Se encontrou, armazena o Transform dele na nossa vari�vel 'totemAlvo'.
            totemAlvo = totemObject.transform;
        }
        else
        {
            // Se n�o encontrou, envia um erro claro no Console do Unity.
            // Isso ajuda MUITO a descobrir problemas.
            Debug.LogError("ERRO: O Totem Principal com a tag 'TotemPrincipal' n�o foi encontrado na cena! Verifique se a tag est� correta no objeto do Totem.");
        }
    }

    // Update � chamado a cada frame
    void Update()
    {
        // Se, por algum motivo, n�o temos um alvo, n�o fazemos nada.
        if (totemAlvo == null)
        {
            return;
        }

        // A l�gica de movimento continua a mesma, pois � muito eficiente.
        transform.position = Vector2.MoveTowards(transform.position, totemAlvo.position, velocidade * Time.deltaTime);

        // L�gica opcional para virar o sprite
        if (transform.position.x > totemAlvo.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (transform.position.x < totemAlvo.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}