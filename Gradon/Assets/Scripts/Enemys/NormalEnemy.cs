using UnityEngine;

public class NormalEnemy : Enemy
{
    [Header("Configura��o Extra")]
    [SerializeField] private float speedMultiplier = 1.5f; // aumenta a velocidade
    [SerializeField] private int maxHealthExtra = 100;    // vida aumentada

    private int currentHealthExtra;

    protected override void Start()
    {
        base.Start();
        currentHealthExtra = maxHealthExtra; // define vida do inimigo
    }

    void Update()
    {
        if (Totem.instance == null || IsDead) return;

        // Movimenta mais r�pido em dire��o ao Totem
        Vector3 direction = (Totem.instance.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * speed * speedMultiplier;

        // Ataque se estiver pr�ximo do Totem
        float distance = Vector3.Distance(transform.position, Totem.instance.transform.position);
        if (distance <= 1f)
        {
            Attack();
        }
    }

    public override void Attack()
    {
        if (Totem.instance != null)
        {
            Totem.instance.TakeDamage(10); // dano ao Totem
            Debug.Log("NormalEnemy atacou corpo a corpo!");
        }
    }

    // Fun��o para receber dano das torres
    public void TakeDamage(int damage)
    {
        currentHealthExtra -= damage;
        if (currentHealthExtra <= 0)
        {
            Die();
        }
    }
}
