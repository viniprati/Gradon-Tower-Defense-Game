using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [Tooltip("A velocidade com que o Boss se move em direção ao Totem.")]
    [SerializeField] private float velocidade = 2f;

    // A referência para o Transform do Totem.
    // Agora é privada, pois o script vai preenchê-la sozinho.
    private Transform totemAlvo;

    // A função Start é chamada uma única vez quando o Boss é criado.
    // É o lugar perfeito para encontrar o alvo.
    void Start()
    {
        // 1. ENCONTRAR O TOTEM AUTOMATICAMENTE:
        // Procura por qualquer GameObject na cena que tenha a tag "TotemPrincipal".
        GameObject totemObject = GameObject.FindGameObjectWithTag("TotemPrincipal");

        // 2. VERIFICAR SE O TOTEM FOI ENCONTRADO:
        if (totemObject != null)
        {
            // Se encontrou, armazena o Transform dele na nossa variável 'totemAlvo'.
            totemAlvo = totemObject.transform;
        }
        else
        {
            // Se não encontrou, envia um erro claro no Console do Unity.
            // Isso ajuda MUITO a descobrir problemas.
            Debug.LogError("ERRO: O Totem Principal com a tag 'TotemPrincipal' não foi encontrado na cena! Verifique se a tag está correta no objeto do Totem.");
        }
    }

    // Update é chamado a cada frame
    void Update()
    {
        // Se, por algum motivo, não temos um alvo, não fazemos nada.
        if (totemAlvo == null)
        {
            return;
        }

        // A lógica de movimento continua a mesma, pois é muito eficiente.
        transform.position = Vector2.MoveTowards(transform.position, totemAlvo.position, velocidade * Time.deltaTime);

        // Lógica opcional para virar o sprite
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