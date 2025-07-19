using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;
    public GameObject itemPrefab;            // UI prefab (must be a RectTransform)
    public Transform layoutGroupParent;      // Parent with HorizontalLayoutGroup

    public Transform BosslayoutGroupParent;      // Parent with HorizontalLayoutGroup
    public Transform PlayerlayoutGroupParent;      // Parent with HorizontalLayoutGroup


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SpawnItem(Card chosencard, bool isboss)
    {
        if (itemPrefab == null || layoutGroupParent == null)
        {
            Debug.LogWarning("Prefab or LayoutGroup not assigned.");
            return;
        }

        if (isboss)
        {
            CreateCard(itemPrefab, BosslayoutGroupParent, chosencard, false, true);
        }
        else
        {
            CreateCard(itemPrefab, layoutGroupParent, chosencard, false, false);
        }

    }
    public void SpawnFightCardItem(Card chosencard)
    {
        if (itemPrefab == null || layoutGroupParent == null)
        {
            Debug.LogWarning("Prefab or LayoutGroup not assigned.");
            return;
        }

        CreateCard(itemPrefab, PlayerlayoutGroupParent, chosencard, true, false);
    }

    public void CreateCard(GameObject itemPF, Transform LayoutGroup, Card Chosen, bool isZoomable, bool isBoss)
    {
        var CCard = Instantiate(itemPF, LayoutGroup);
        CCard.GetComponent<CardDisplay>().Card = Chosen;
        CCard.GetComponent<CardDisplay>().Card.name = Chosen.name;
        CCard.GetComponent<CardDisplay>().Card.type = Chosen.type;
        CCard.GetComponent<CardDisplay>().Card.actionType = Chosen.actionType;
        CCard.GetComponent<CardDisplay>().Card.artwork = Chosen.artwork;
        CCard.GetComponent<CardDisplay>().Card.value1 = Chosen.value1;
        CCard.GetComponent<CardDisplay>().Card.value2 = Chosen.value2;
        CCard.GetComponent<CardDisplay>().Card.value3 = Chosen.value3;

        if (isZoomable)
        {
            CCard.GetComponent<Zoom>().enabled = true;
            CCard.GetComponent<BoxCollider2D>().enabled = true;
        }

    }
}