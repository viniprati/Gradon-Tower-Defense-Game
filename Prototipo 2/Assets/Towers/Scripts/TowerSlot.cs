// TowerSlot.cs
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    // Variável para rastrear se o slot tem uma torre.
    // 'public' para que o PlayerController possa ler o valor.
    // 'private set' para que somente este script possa alterar o valor (mais seguro).
    public bool isOccupied { get; private set; } = false;

    // Referência para a torre que está neste slot.
    private GameObject currentTowerInstance;

    // Método público que o PlayerController irá chamar.
    // Ele recebe o "molde" (prefab) da torre a ser construída.
    public void PlaceTower(GameObject towerPrefab)
    {
        // Checagem de segurança: se o slot já estiver ocupado, não faz nada.
        if (isOccupied)
        {
            Debug.LogWarning("Tentativa de construir em um slot já ocupado. Ação ignorada.");
            return;
        }

        // Cria uma instância (uma cópia) do prefab da torre.
        // Onde? Na posição deste slot (transform.position).
        // Com qual rotação? Nenhuma (Quaternion.identity).
        currentTowerInstance = Instantiate(towerPrefab, transform.position, Quaternion.identity);

        // Marca o slot como ocupado para que outra torre não possa ser construída aqui.
        isOccupied = true;
    }

    // (Opcional) Método para liberar o slot se a torre for destruída.
    // Você pode chamar isso se, no futuro, implementar uma forma de remover/vender torres.
    public void FreeUpSlot()
    {
        isOccupied = false;
        currentTowerInstance = null;
    }

    // Exemplo de como a torre poderia se auto-reportar como destruída (para o futuro):
    // Se a torre tiver um script que a destrói, ela poderia chamar:
    // FindObjectOfType<TowerSlot>().FreeUpSlot(); (não é a melhor forma, mas é um exemplo)
}