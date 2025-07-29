// NormalEnemy.cs
using UnityEngine;

public class NormalEnemy : EnemyBase
{
    // A única coisa que ele precisa fazer é definir como se mover.
    protected override Vector2 HandleMovement()
    {
        // Apenas retorna a direção para o jogador. A classe base cuidará da velocidade.
        if (playerTransform != null)
        {
            return (playerTransform.position - transform.position).normalized;
        }
        return Vector2.zero;
    }
}