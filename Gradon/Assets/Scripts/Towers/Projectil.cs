// Projectile.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    [SerializeField] private float speed = 20f;
    // [SerializeField] private float damage = 10f; // REMOVA OU COMENTE ESTA LINHA
    [SerializeField] private float lifetime = 3f;

    [Header("Identificação do Alvo")]
    [SerializeField] private string targetTag;

    [Header("Efeitos")]
    [SerializeField] private GameObject hitEffectPrefab;

    // --- Variáveis Internas ---
    private Rigidbody2D rb;
    private float currentDamage; // NOVA VARIÁVEL: Armazena o dano para este projétil específico

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // MÉTODO 'LAUNCH' MODIFICADO
    // Agora ele aceita a direção e o valor do dano
    public void Launch(Vector2 direction, float damageFromAttacker)
    {
        // 1. Armazena o dano recebido do atirador
        this.currentDamage = damageFromAttacker;

        // 2. Define a velocidade na direção fornecida
        rb.linearVelocity = direction.normalized * speed;

        // 3. Gira o projétil
        transform.right = direction.normalized;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            IDamageable damageableObject = other.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                // USA A NOVA VARIÁVEL 'currentDamage'
                damageableObject.TakeDamage(currentDamage);

                if (hitEffectPrefab != null)
                {
                    Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }
        }
    }
}