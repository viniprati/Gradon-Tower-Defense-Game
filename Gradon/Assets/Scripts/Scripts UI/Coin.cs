// Coin.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [Header("Configura��es da Moeda")]
    [Tooltip("A quantidade de mana que esta moeda concede ao ser coletada.")]
    [SerializeField] private int manaValue = 5;

    [Tooltip("Opcional: Efeito de part�cula a ser instanciado ao coletar.")]
    [SerializeField] private GameObject collectionEffectPrefab;

    [Tooltip("Opcional: Som a ser tocado ao coletar.")]
    [SerializeField] private AudioClip collectionSound;

    [Header("Comportamento (Opcional)")]
    [Tooltip("Se marcado, a moeda ser� 'sugada' pelo jogador quando ele se aproximar.")]
    [SerializeField] private bool hasMagnetEffect = true;
    [SerializeField] private float magnetRange = 3f;
    [SerializeField] private float magnetSpeed = 8f;

    // --- Vari�veis Internas ---
    private Transform playerTransform;
    private bool isFollowingPlayer = false;

    void Start()
    {
        // Garante que o colisor seja um trigger para n�o ter colis�es f�sicas
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Update()
    {
        // Se o efeito de �m� estiver ativo e a moeda ainda n�o estiver seguindo...
        if (hasMagnetEffect && !isFollowingPlayer)
        {
            // Tenta encontrar o jogador apenas uma vez para otimiza��o
            if (playerTransform == null)
            {
                Totem player = FindFirstObjectByType<Totem>();
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }

            // Se o jogador foi encontrado e est� perto o suficiente...
            if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= magnetRange)
            {
                // ...ativa o movimento em dire��o a ele.
                isFollowingPlayer = true;
            }
        }

        // Se a moeda foi ativada para seguir o jogador...
        if (isFollowingPlayer && playerTransform != null)
        {
            // ...move-se em dire��o a ele.
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, magnetSpeed * Time.deltaTime);
        }
    }

    // Este m�todo � chamado quando outro Collider2D entra no trigger da moeda.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Tenta pegar o script PlayerController do objeto
            Totem player = other.GetComponent<Totem>();

            // Se o script foi encontrado, a coleta � bem-sucedida
            if (player != null)
            {
                Collect(player);
            }
        }
    }

    private void Collect(Totem player)
    {
        // 1. Adiciona mana ao jogador
        player.AddMana(manaValue);

        // 2. Toca o som de coleta (se houver um configurado)
        if (collectionSound != null)
        {
            // AudioSource.PlayClipAtPoint cria uma fonte de �udio tempor�ria na posi��o,
            // toca o som e se destr�i. Perfeito para efeitos que n�o precisam de um objeto persistente.
            AudioSource.PlayClipAtPoint(collectionSound, transform.position);
        }

        // 3. Instancia o efeito visual (se houver um)
        if (collectionEffectPrefab != null)
        {
            Instantiate(collectionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 4. Destr�i o objeto da moeda
        Destroy(gameObject);
    }

    // Desenha o raio do �m� no editor para facilitar o balanceamento
    private void OnDrawGizmosSelected()
    {
        if (hasMagnetEffect)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, magnetRange);
        }
    }
}