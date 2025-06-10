using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<string> items = new List<string>();
    [SerializeField] List<TextMeshProUGUI> inventory_txt = new List<TextMeshProUGUI>();

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log("Added to inventory: " + itemName);
        inventory_txt[items.Count - 1].text = itemName;
    }

    public void PrintInventory()
    {
        Debug.Log("Inventory contains: " + string.Join(", ", items));
    }

    public void DisplayInventory()
    {
        Debug.Log("Displaying Inventory:");
        foreach (string item in items)
        {
            Debug.Log("- " + item);
            
        }
    }
}
