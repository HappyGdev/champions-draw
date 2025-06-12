using UnityEngine;

public class UiItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;            // UI prefab (must be a RectTransform)
    public Transform layoutGroupParent;      // Parent with HorizontalLayoutGroup

    public void SpawnItem(Card chosencard)
    {
        if (itemPrefab == null || layoutGroupParent == null)
        {
            Debug.LogWarning("Prefab or LayoutGroup not assigned.");
            return;
        }

        var CCard= Instantiate(itemPrefab, layoutGroupParent);
        CCard.GetComponent<CardDisplay>().Card = chosencard;
        CCard.GetComponent<CardDisplay>().Card.name = chosencard.name;
        CCard.GetComponent<CardDisplay>().Card.type = chosencard.type;
        CCard.GetComponent<CardDisplay>().Card.actionType = chosencard.actionType;
        CCard.GetComponent<CardDisplay>().Card.artwork = chosencard.artwork;
        CCard.GetComponent<CardDisplay>().Card.value1 = chosencard.value1;
        CCard.GetComponent<CardDisplay>().Card.value2 = chosencard.value2;
        CCard.GetComponent<CardDisplay>().Card.value3 = chosencard.value3;

    }

}
