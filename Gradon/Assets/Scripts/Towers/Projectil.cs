// Projectile.cs (Modificado para Tiro Reto / Não-Teleguiado)

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    [Tooltip("A velocidade com que o projétil se move.")]
    [SerializeField] private float speed = 20f;
    [Tooltip("Tempo em segundos antes que o projétil se destrua, caso não acerte nada.")]
    [SerializeField] private float lifetime = 3f;

    [Header("Efeitos")]
    [Tooltip("Prefab do efeito a ser criado no momento do impacto (opcional).")]
    [SerializeField] private GameObject hitEffectPrefab;

    // --- MUDANÇA #1: Variável de tag para colisão ---
    // Adicionamos uma tag para tornar a colisão mais flexível.
    [Header("Identificação do Alvo")]
    [SerializeField] private string enemyTag = "Enemy";

    // --- Variáveis Internas ---
    private Rigidbody2D rb;
    private int currentDamage;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Start()
    {
        // Agenda a destruição do projétil após 'lifetime' segundos
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Método público que a torre chama para configurar e DISPARAR o projétil.
    /// </summary>
    public void Seek(Transform _target, int damageFromAttacker)
    {
        // --- MUDANÇA #2: LÓGICA DE DISPARO MOVIDA PARA CÁ ---

        // 1. Armazena o dano que o projétil causará.
        this.currentDamage = damageFromAttacker;

        // 2. Calcula a direção para o alvo UMA ÚNICA VEZ.
        Vector2 direction = (_target.position - transform.position).normalized;

        // 3. Define a velocidade do Rigidbody nessa direção fixa.
        // A partir daqui, o motor de física da Unity cuidará do movimento.
        rb.linearVelocity = direction * speed;

        // 4. (Opcional) Rotaciona o projétil para "olhar" na direção do disparo.
        transform.up = direction;
    }

    // --- MUDANÇA #3: REMOÇÃO DA LÓGICA DE PERSEGUIÇÃO ---
    // O método FixedUpdate() foi completamente removido, pois não precisamos mais
    // recalcular a direção do projétil a cada frame.

    /// <summary>
    /// Chamado quando o colisor do projétil entra em contato com outro.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // --- MUDANÇA #4: DETECÇÃO DE COLISÃO MELHORADA ---
        // Em vez de checar por um alvo específico, agora checamos se o objeto
        // atingido tem a tag de inimigo. Isso permite que o tiro acerte
        // qualquer inimigo que entrar no seu caminho.
        if (other.CompareTag(enemyTag))
        {
            // Tenta pegar o script do inimigo para aplicar dano.
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(currentDamage);
            }

            // Cria um efeito de impacto visual, se um prefab foi configurado.
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            // Destrói o projétil após o impacto.
            Destroy(gameObject);
        }
    }
}