using UnityEngine;

// A parte mais importante é ": Enemy".
// Isso significa que a classe Boss é um tipo de Enemy e herda todas as suas variáveis e métodos.
public class Boss : Enemy
{
    // --- ESTÁ PRONTO! ---

    // O script pode ficar vazio por enquanto. Toda a lógica de encontrar o totem,
    // mover-se, verificar o alcance e atacar já está no script "Enemy.cs".

    // Se no futuro você quiser que o Boss tenha um comportamento especial
    // (por exemplo, um ataque em área), você pode adicionar o código aqui.
    // Por exemplo, para fazer o Boss gritar ao nascer:
    /*
    protected override void Start()
    {
        base.Start(); // 'base.Start()' executa o código da classe Enemy
        Debug.Log("O BOSS APARECEU!"); // Este é um comportamento extra, só do Boss
    }
    */
}