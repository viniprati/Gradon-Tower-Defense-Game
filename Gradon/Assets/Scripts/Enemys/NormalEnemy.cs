using UnityEngine;

public class NormalEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (Totem.instance == null || IsDead) return;

        Vector3 direction = (Totem.instance.transform.position - transform.position).normalized;
        rb.velocity = direction * speed;

        // Ataque se estiver próximo do Totem
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
            Totem.instance.TakeDamage(10); // dano padrão
            Debug.Log("NormalEnemy atacou corpo a corpo!");
        }
    }
}
