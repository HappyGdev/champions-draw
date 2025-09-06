using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour
{
    private string coins5k = "coin5k";
    private string coins10k = "coin10k";
    private string coins50k = "coin50k";
    private string coins5100k = "coin5100k";
    private string removeAds = "removeads";
    public ShopManager shopController;

    [SerializeField]
    private Button coins5kButton;
    [SerializeField]
    private Button coins10kButton;
    [SerializeField]
    private Button coins50kButton;
    [SerializeField]
    private Button coins5100kButton;
    [SerializeField]
    private Button removeAdsButton;

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id.Equals("coin5k"))
        {
            Debug.Log("Add 100 money");
            shopController.Coin5kButton();
        }
        else if (product.definition.id.Equals("coin10k"))// == coins500)
        {
            Debug.Log("Add 500 money");
            shopController.Coin10kButton();
        }
        else if (product.definition.id.Equals("coin50k"))// == coins500)
        {
            Debug.Log("Add 500 money");
            shopController.Coin50kButton();
        }
        else if (product.definition.id.Equals("coin5100k"))// == coins500)
        {
            Debug.Log("Add 500 money");
            shopController.Coin5100kButton();
        }
        else if (product.definition.id == removeAds)
        {
            shopController.RemoveAdsButton();
        }
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log(product.definition.id + "Purchase Failure Reason" + failureDescription);
    }
    public void OnProductFetched(Product product)
    {
        if (product.definition.id == coins5k)
        {
            UpdateButtonPrice(coins5kButton, product);
        }
        else if(product.definition.id== coins10k)
        {
            UpdateButtonPrice(coins10kButton, product);
        }
        else if (product.definition.id == coins50k)
        {
            UpdateButtonPrice(coins50kButton, product);
        }
        else if (product.definition.id == coins5100k)
        {
            UpdateButtonPrice(coins5100kButton, product);
        }
        else if (product.definition.id == removeAds)
        {
            UpdateButtonPrice(removeAdsButton, product);
        }
    }
    private void UpdateButtonPrice(Button button, Product product)
    {
        TextMeshProUGUI buttonText=button.GetComponent<TextMeshProUGUI>();
        if (buttonText !=null)
        {
            buttonText.text=product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
        }
    }
}
