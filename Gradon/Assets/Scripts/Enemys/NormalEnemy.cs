// NormalEnemy.cs
using UnityEngine;

public class NormalEnemy : Enemy // Garante que est� herdando da classe base correta
{
    // A �nica coisa que ele precisa fazer � definir como se mover.
    protected override Vector2 HandleMovement()
    {
        // CORRE��O: Usa a vari�vel 'currentTarget' herdada da classe base.
        if (currentTarget != null)
        {
            // A classe base j� calcula a dire��o em 'moveDirection', ent�o podemos simplesmente retorn�-la.
            return moveDirection;
        }

        // Se, por algum motivo, n�o houver alvo, ele fica parado.
        return Vector2.zero;
    }

    // Voc� precisar� adicionar uma l�gica de ataque para o inimigo normal (Ghoul) aqui
    // Exemplo:
    /*
    private void Update()
    {
        base.Update(); // Roda o Update da classe base
        
        // Se estiver perto o suficiente do alvo, ataca
        if (currentTarget != null && Vector2.Distance(transform.position, currentTarget.position) <= attackRange)
        {
            // L�gica de ataque com cooldown aqui...
        }
    }
    */
}