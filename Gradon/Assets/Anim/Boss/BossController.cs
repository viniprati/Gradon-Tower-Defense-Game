using UnityEngine;

// A parte mais importante � ": Enemy".
// Isso significa que a classe Boss � um tipo de Enemy e herda todas as suas vari�veis e m�todos.
public class Boss : Enemy
{
    // --- EST� PRONTO! ---

    // O script pode ficar vazio por enquanto. Toda a l�gica de encontrar o totem,
    // mover-se, verificar o alcance e atacar j� est� no script "Enemy.cs".

    // Se no futuro voc� quiser que o Boss tenha um comportamento especial
    // (por exemplo, um ataque em �rea), voc� pode adicionar o c�digo aqui.
    // Por exemplo, para fazer o Boss gritar ao nascer:
    /*
    protected override void Start()
    {
        base.Start(); // 'base.Start()' executa o c�digo da classe Enemy
        Debug.Log("O BOSS APARECEU!"); // Este � um comportamento extra, s� do Boss
    }
    */
}