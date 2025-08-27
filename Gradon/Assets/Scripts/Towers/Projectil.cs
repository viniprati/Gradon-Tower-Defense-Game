// Projectile.cs

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Configura��es do Proj�til")]
    [Tooltip("A velocidade com que o proj�til se move.")]
    [SerializeField] private float speed = 20f;
    [Tooltip("Tempo em segundos antes que o proj�til se destrua, caso n�o acerte nada.")]
    [SerializeField] private float lifetime = 3f;

    [Header("Efeitos")]
    [Tooltip("Prefab do efeito a ser criado no momento do impacto (opcional).")]
    [SerializeField] private GameObject hitEffectPrefab;

    // --- Vari�veis Internas ---
    private Rigidbody2D rb;
    private Transform target;      // O alvo que o proj�til deve perseguir
    private int currentDamage;     // O dano que este proj�til espec�fico ir� causar

    void Awake()
    {
        // Pega a refer�ncia do componente de f�sica do proj�til
        rb = GetComponent<Rigidbody2D>();
        // Garante que o colisor do proj�til est� configurado como 'Trigger'
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Start()
    {
        // Agenda a destrui��o do proj�til ap�s 'lifetime' segundos
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// M�todo p�blico que a torre chama para configurar o proj�til com suas "ordens".
    /// </summary>
    /// <param name="_target">O Transform do inimigo que deve ser perseguido.</param>
    /// <param name="damageFromAttacker">A quantidade de dano a ser aplicada no impacto.</param>
    public void Seek(Transform _target, int damageFromAttacker)
    {
        this.target = _target;
        this.currentDamage = damageFromAttacker;
    }

    // FixedUpdate � chamado em um intervalo de tempo fixo, ideal para f�sica.
    void FixedUpdate()
    {
        // Se o alvo for destru�do no meio do caminho, o proj�til se autodestr�i.
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // --- L�gica de Persegui��o (Homing) ---
        // 1. Calcula a dire��o do proj�til at� a posi��o atual do alvo
        Vector2 direction = (target.position - transform.position).normalized;
        // 2. Aplica a velocidade ao Rigidbody nessa dire��o
        rb.linearVelocity = direction * speed;
        // 3. (Opcional) Rotaciona o sprite do proj�til para "olhar" na dire��o do movimento
        transform.up = direction;
    }

    // Chamado quando este colisor (marcado como 'Is Trigger') entra em contato com outro.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto com o qual colidimos � de fato o nosso alvo.
        if (other.transform == target)
        {
            // Tenta pegar um script no alvo que possa receber dano.
            // (Certifique-se que seu inimigo tenha um script com este m�todo!)
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

            // Destr�i o proj�til ap�s o impacto.
            Destroy(gameObject);
        }
    }
}