using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int price;
    public bool isUnlocked;
    public Button button;
    public TextMeshProUGUI priceText;
    public Image avatar;
    public GameObject lockIco;

}
public class ShopManager : MonoBehaviour
{
    public int playerCoins = 100;  
    public TextMeshProUGUI coinText;

    public ShopItem[] shopItems;
    public GameObject userPic;
    void Start()
    {
        UpdateUI();

        for (int i = 0; i < shopItems.Length; i++)
        {
            int index = i; 
            shopItems[i].button.onClick.AddListener(() => OnItemClick(index));
        }
    }

    void UpdateUI()
    {
        coinText.text = "Coins: " + playerCoins;

        for (int i = 0; i < shopItems.Length; i++)
        {
            if (i == 0)
            {
                shopItems[i].isUnlocked = true; // first Item Unlocked
                shopItems[i].lockIco.SetActive(false);
            }

            if (shopItems[i].isUnlocked)
            {
                shopItems[i].priceText.text = " ";
                shopItems[i].button.interactable = true;
                shopItems[i].lockIco.SetActive(false);    
            }
            else
            {
                shopItems[i].priceText.text = shopItems[i].price.ToString();
                // if we have Enough Money Enabled
                shopItems[i].button.interactable = playerCoins >= shopItems[i].price;
                shopItems[i].lockIco.SetActive(true);
            }
        }
    }

    void OnItemClick(int index)
    {
        ShopItem item = shopItems[index];

        if (!item.isUnlocked)
        {
            if (playerCoins >= item.price)
            {
                playerCoins -= item.price;
                item.isUnlocked = true;
                Debug.Log(item.itemName + " Sold!");

                // ذخیره در Firebase
                FirebaseManager.Instance.SaveUnlockedItems(shopItems);
            }
            else
            {
                Debug.Log("Don't Have Enough Money!");
                return;
            }
        }

        // انتخاب آیتم
        Debug.Log("Item Selected: " + index);
        PlayerPrefs.SetInt("UserProfileNumnber", index); // آیتم انتخاب شده
        FirebaseManager.Instance.SaveUserProfileNumber(index); // سیو انتخاب کاربر
        UpdateUserPhoto();
        UpdateUI();
    }


    public void UpdateUserPhoto()
    {
        var userprofilenum = PlayerPrefs.GetInt("UserProfileNumnber");
        userPic.GetComponent<Image>().sprite = shopItems[userprofilenum].avatar.sprite;
    }
    public void LoadUnlockedItems(int[] unlocked)
    {
        for (int i = 0; i < shopItems.Length && i < unlocked.Length; i++)
        {
            shopItems[i].isUnlocked = unlocked[i] == 1;
            Debug.Log("Vazaiate item : " + i + "  " + shopItems[i].isUnlocked);
        }
        UpdateUI();
    }

}
