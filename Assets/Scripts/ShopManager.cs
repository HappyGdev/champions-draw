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
    public TextMeshProUGUI shopCoinText;

    public ShopItem[] shopItems;
    public GameObject userPic;
    [Header ("badge")]
    public GameObject[] badges;


    void Start()
    {
        UpdateUI();

        for (int i = 0; i < shopItems.Length; i++)
        {
            int index = i; 
            shopItems[i].button.onClick.AddListener(() => OnItemClick(index));
        }
    }
    public void ResetShop()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            if (i == 0)
            {
                shopItems[i].isUnlocked = true; // first Item Unlocked
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
        foreach (var bdg in badges)
        {
            bdg.SetActive(false);
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

                FirebaseManager.Instance.SaveUnlockedItems(shopItems);
                UpdateUserPhoto(index);
            }
            else
            {
                Debug.Log("Don't Have Enough Money!");
                return;
            }
        }

        //Debug.Log("Item Selected: " + index);
        //PlayerPrefs.SetInt("UserProfileNumnber", index); 
        //FirebaseManager.Instance.SaveUserProfileNumber(index); 
        UpdateUI();
    }


    public void UpdateUserPhoto(int index)
    {
        //var userprofilenum = PlayerPrefs.GetInt("UserProfileNumnber");
        userPic.GetComponent<Image>().sprite = shopItems[index].avatar.sprite;
    }
    public void LoadUnlockedItems(int[] unlocked)
    {
        for (int i = 0; i < shopItems.Length && i < unlocked.Length; i++)
        {
            shopItems[i].isUnlocked = unlocked[i] == 1;
        }
        UpdateUI();
    }

    public void Coin100Button()
    {
        Debug.Log("Coin 100");
        AddCoin(100);
    }
    public void Coin500Button()
    {
        Debug.Log("Coin 500");
        AddCoin(500);
    }
    public void RemoveAdsButton()
    {
        Debug.Log("Remove Ads");
    }

    //public void AddCoin(int coinAmounts)
    //{
    //    int currentValue;
    //    if(int.TryParse(shopCoinText.text, out currentValue))
    //    {
    //        currentValue += coinAmounts;
    //        shopCoinText.text=currentValue.ToString();   
    //    }
    //}
    public void AddCoin(int coinAmounts)
    {
        playerCoins += coinAmounts;
        shopCoinText.text = playerCoins.ToString();
    }
    public void RemoveAds()
    {
    }
    public void LoadBadges(int num,int badge)
    {
        if (num == 1)
        {
            badges[badge].SetActive(true);
        }
        else
        {
            badges[badge].SetActive(false);
        }
    }
}
