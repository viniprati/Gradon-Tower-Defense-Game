// Projectile.cs (Versão Detetive)

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Efeitos")]
    [SerializeField] private GameObject hitEffectPrefab;

    private Rigidbody2D rb;
    private int currentDamage;
    private bool hasHit = false; // Garante que o projétil acerte apenas uma vez

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Seek(Transform _target, int damageFromAttacker)
    {
        this.currentDamage = damageFromAttacker;
        if (_target == null) { Destroy(gameObject); return; }

        Vector2 direction = (_target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
        transform.up = direction;
    }

    /// <summary>
    /// O método de detecção de colisão, agora com um relatório completo.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // Se já acertamos algo, ignora
        if (hasHit) return;

        // --- INÍCIO DO RELATÓRIO DO DETETIVE ---

        // 1. O projétil anuncia que tocou em algo.
        Debug.Log($"RELATÓRIO DE IMPACTO: Projétil tocou em '{other.gameObject.name}'.");

        // 2. O projétil verifica a tag do objeto.
        if (other.CompareTag(enemyTag))
        {
            // Se a tag bate, ele anuncia.
            Debug.Log($"<color=yellow>VERIFICAÇÃO DE TAG: SUCESSO! O objeto '{other.name}' tem a tag '{enemyTag}'.</color>");

            // 3. O projétil tenta encontrar o script 'Enemy' no objeto.
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Se o script for encontrado, ele anuncia o sucesso final e aplica o dano.
                Debug.Log($"<color=green>VERIFICAÇÃO DE SCRIPT: SUCESSO! Aplicando {currentDamage} de dano.</color>");

                hasHit = true; // Marca que já acertou
                enemy.TakeDamage(currentDamage); // Causa o dano

                // Lógica de impacto
                if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject); // Destrói o projétil
            }
            else
            {
                // Se a tag está certa, mas o script não foi encontrado.
                Debug.LogError($"<color=red>FALHA CRÍTICA!</color> O objeto '{other.name}' tem a tag 'Enemy', mas o script 'Enemy.cs' (ou um de seus filhos) não foi encontrado nele. Verifique o prefab do inimigo.");
                hasHit = true;
                Destroy(gameObject);
            }
        }
        else
        {
            // Se a tag não bate.
            Debug.Log($"VERIFICAÇÃO DE TAG: FALHOU. A tag do objeto é '{other.tag}', não '{enemyTag}'. Ignorando colisão.");
        }
    }
}