using UnityEngine;

public class NormalEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        rb.velocity = Vector2.left * speed;
    }

    public override void Attack()
    {
        Debug.Log("NormalEnemy atacou corpo a corpo!");
    }
}
