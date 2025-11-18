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
        // Garante que o projétil se destrua se não acertar nada
        Destroy(gameObject, lifetime);
    }

    public void Seek(Transform _target, int damageFromAttacker)
    {
        this.currentDamage = damageFromAttacker;
        if (_target == null) { Destroy(gameObject); return; }

        Vector2 direction = (_target.position - transform.position).normalized;
        rb.velocity = direction * speed; // Usando rb.velocity para movimento baseado em física
        transform.up = direction; // Rotaciona o projétil para encarar o alvo
    }

    // --- DETECTOR DE DANO INTEGRADO ---
    // Este método foi modificado para nos dar um relatório completo no Console.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Se este projétil já acertou alguma coisa, ele não faz mais nada.
        if (hasHit) return;

        // --- INÍCIO DO RELATÓRIO DO DETETIVE ---

        // 1. O projétil anuncia que colidiu com algo.
        Debug.Log($"[DETECTOR] Projétil colidiu com o objeto '{other.gameObject.name}'.");

        // 2. O projétil verifica se a tag do objeto é a que procuramos ("Enemy").
        if (other.CompareTag(enemyTag))
        {
            // Se a tag corresponde, ele anuncia o sucesso.
            Debug.Log($"<color=yellow>[DETECTOR - TAG OK]</color> O objeto '{other.name}' tem a tag '{enemyTag}'. Verificando script...");

            // 3. O projétil tenta encontrar um script que herda de 'Enemy' no objeto.
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Se o script for encontrado, anuncia o sucesso final e aplica o dano.
                Debug.Log($"<color=green>[DETECTOR - SUCESSO TOTAL]</color> Script 'Enemy' encontrado! Aplicando {currentDamage} de dano.");

                hasHit = true; // Marca que já acertou para evitar hits duplos
                enemy.TakeDamage(currentDamage); // Causa o dano

                // Cria o efeito de impacto e destrói o projétil
                if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                // Se a tag está certa, mas o script não foi encontrado.
                Debug.LogError($"<color=red>[DETECTOR - ERRO CRÍTICO]</color> O objeto '{other.name}' tem a tag '{enemyTag}', MAS NÃO TEM um script que herda de 'Enemy' (como BossController ou NormalEnemy). Verifique o prefab do inimigo!");
                hasHit = true; // Marca como hit para não continuar verificando
                Destroy(gameObject); // Destrói o projétil
            }
        }
        else
        {
            // Se a tag não corresponde, ele avisa e ignora.
            Debug.LogWarning($"[DETECTOR - TAG ERRADA] A tag do objeto é '{other.tag}', não '{enemyTag}'. O projétil vai passar direto.");
        }
    }
}