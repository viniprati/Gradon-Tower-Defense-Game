// BuildUIController.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildUIController : MonoBehaviour
{
    [Header("Referências de UI")]
    [SerializeField] private GameObject selectionPanel; // O painel principal que contém tudo
    [SerializeField] private Image[] towerSlots;       // Slots de imagem para os ícones das torres
    [SerializeField] private GameObject selectionArrow; // A seta que aponta para o slot selecionado

    private List<GameObject> availableTowers;
    private int currentIndex = -1;
    private RectTransform arrowRectTransform;

    void Awake()
    {
        arrowRectTransform = selectionArrow.GetComponent<RectTransform>();
        // Começa com a UI desligada
        selectionPanel.SetActive(false);
    }

    // Chamado pelo PlayerController para INICIAR a seleção
    public void ShowSelection(List<GameObject> towers, int startIndex)
    {
        this.availableTowers = towers;
        selectionPanel.SetActive(true);

        // Preenche os slots com os ícones das torres
        for (int i = 0; i < towerSlots.Length; i++)
        {
            if (i < towers.Count)
            {
                towerSlots[i].gameObject.SetActive(true);
                towerSlots[i].sprite = towers[i].GetComponent<TowerBase>().towerIcon;
            }
            else
            {
                // Esconde slots não utilizados
                towerSlots[i].gameObject.SetActive(false);
            }
        }

        // Define o índice inicial
        UpdateSelection(startIndex);
    }

    // Chamado pelo PlayerController para ATUALIZAR a seleção
    public void UpdateSelection(int newIndex)
    {
        if (newIndex < 0 || newIndex >= availableTowers.Count) return;

        currentIndex = newIndex;

        // Move a seta para a posição do slot selecionado
        Vector2 targetPosition = towerSlots[currentIndex].GetComponent<RectTransform>().anchoredPosition;
        arrowRectTransform.anchoredPosition = new Vector2(targetPosition.x - 2, arrowRectTransform.anchoredPosition.y);
    }

    // Chamado pelo PlayerController para FINALIZAR a seleção
    public void HideSelection()
    {
        selectionPanel.SetActive(false);
    }
}