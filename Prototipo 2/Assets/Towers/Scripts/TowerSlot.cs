// TowerSlot.cs (Nova Versão)
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    // A única variável que precisamos: um booleano para saber se o slot está livre.
    // 'private set' significa que só este script pode mudar o valor de 'isOccupied',
    // mas outros scripts podem ler (verificar) se ele é 'true' ou 'false'.
    public bool isOccupied { get; private set; }

    // Função para construir uma torre.
    // Recebe o "molde" (prefab) da torre que deve ser construída.
    public void PlaceTower(GameObject towerPrefab)
    {
        // Checagem de segurança: se o slot já estiver ocupado, não faz nada.
        if (isOccupied)
        {
            Debug.Log("Tentativa de construir em um slot já ocupado.");
            return;
        }

        // AQUI A MÁGICA ACONTECE:
        // Cria uma cópia (uma instância) do prefab da torre.
        // Onde? Na posição deste slot (transform.position).
        // Com qual rotação? Nenhuma (Quaternion.identity).
        Instantiate(towerPrefab, transform.position, Quaternion.identity);

        // Marca o slot como ocupado para que outra torre não possa ser construída aqui.
        isOccupied = true;
    }

    // (Opcional) Função para liberar o slot.
    // Você pode chamar isso se, no futuro, implementar uma forma de remover/vender torres.
    public void FreeUpSlot()
    {
        isOccupied = false;
    }
}