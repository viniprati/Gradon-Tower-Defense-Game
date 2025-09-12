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
    }

    public override void Attack()
    {
        Debug.Log("NormalEnemy atacou corpo a corpo!");
        // Aqui você pode adicionar lógica de dano ao Totem
        Totem.instance.TakeDamage(10);
    }
}
