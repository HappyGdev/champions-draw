using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour
{
    private string coins100 = "coins100";
    private string coins500 = "coins500";
    private string removeAds = "removeads";
    public ShopManager shopController;

    [SerializeField]
    private Button coins100Button;
    [SerializeField]
    private Button coins500Button;
    [SerializeField]
    private Button removeAdsButton;

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == coins100)
        {
            shopController.Coin100Button();
        }
        else if (product.definition.id == coins500)
        {
            shopController.Coin500Button();
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
        if (product.definition.id == coins100)
        {
            UpdateButtonPrice(coins100Button, product);
        }
        else if(product.definition.id== coins500)
        {
            UpdateButtonPrice(coins500Button, product);
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
