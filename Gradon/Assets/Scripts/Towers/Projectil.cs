// Projectile.cs (Com linha de Debug para Diagnóstico)

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

    [Header("Identificação do Alvo")]
    [SerializeField] private string enemyTag = "Enemy";

    // Variáveis Internas
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
        this.currentDamage = damageFromAttacker;

        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (_target.position - transform.position).normalized;
        rb.velocity = direction * speed;
        transform.up = direction;
    }

    /// <summary>
    /// Chamado quando o colisor do projétil entra em contato com outro.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // --- ADIÇÃO DE DEBUG AQUI ---
        // Esta linha é nosso "espião". Ela vai nos dizer o nome de TUDO que o projétil tocar.
        Debug.Log($"Projétil tocou em: '{other.gameObject.name}' com a Tag: '{other.tag}'");

        // A lógica original continua. Verificamos se o objeto atingido tem a tag de inimigo.
        if (other.CompareTag(enemyTag))
        {
            // Tenta pegar o script do inimigo para aplicar dano.
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log($"<color=green>Acerto VÁLIDO!</color> Dando {currentDamage} de dano em {other.name}.");
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