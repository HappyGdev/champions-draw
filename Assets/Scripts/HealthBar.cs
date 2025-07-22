using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public static HealthBar instance;

    public Image PlayerhealthBarFill;
    public Image BossHealthBarFIll;
    public float maxHealth = 100f;
    public float PlayercurrentHealth;
    public TextMeshProUGUI Player_Health_text;
    public TextMeshProUGUI Boss_Health_text;
    public float BosscurrentHealth;
    private bool isDoubleDamageRound;

    private void Awake()
    {
        if(instance == null){instance = this;    }
    }

    void Start()
    {
        PlayercurrentHealth = maxHealth;
        BosscurrentHealth = maxHealth;
        UpdatePlayerHealthUI();
        UpdateBossHealthUI();
    }

    public void DoubleDamageRound()
    {
        isDoubleDamageRound=true;   
    }
    public void PlayerTakeDamage(float amount)
    {
        PlayercurrentHealth -= amount;
        PlayercurrentHealth = Mathf.Clamp(PlayercurrentHealth, 0, maxHealth);
        UIAnimationUtility.ShakePosition(PlayerhealthBarFill.rectTransform, new Vector3(1, 10, 1), 0.5f, 10, 90, Ease.InOutBounce);
        UpdatePlayerHealthUI();
    }

    void UpdatePlayerHealthUI()
    {
        if (PlayerhealthBarFill != null) 
        {
            PlayerhealthBarFill.fillAmount = PlayercurrentHealth / maxHealth;
            //Player_Health_text.text = (PlayerhealthBarFill.fillAmount * 100).ToString() + " % ";
            Player_Health_text.text = "100 / " + (PlayercurrentHealth).ToString();
        }
    }

    public void BossTakeDamage(float amount)
    {
        if (isDoubleDamageRound)
        {
            BosscurrentHealth -= amount * 2;
            BosscurrentHealth = Mathf.Clamp(BosscurrentHealth, 0, maxHealth);
            UIAnimationUtility.ShakePosition(BossHealthBarFIll.rectTransform, new Vector3(1, 10, 1), 0.5f, 10, 90, Ease.InOutBounce);
            UpdateBossHealthUI();
            isDoubleDamageRound = false;
        }
        else
        {
            BosscurrentHealth -= amount;
            BosscurrentHealth = Mathf.Clamp(BosscurrentHealth, 0, maxHealth);
            UIAnimationUtility.ShakePosition(BossHealthBarFIll.rectTransform, new Vector3(1, 10, 1), 0.5f, 10, 90, Ease.InOutBounce);
            UpdateBossHealthUI();
        }

    }

    void UpdateBossHealthUI()
    {
        if (BossHealthBarFIll != null)
        {
            BossHealthBarFIll.fillAmount = BosscurrentHealth / maxHealth;
            //Boss_Health_text.text = (BossHealthBarFIll.fillAmount*100).ToString() + " % ";
            Boss_Health_text.text = "100 / " + (BosscurrentHealth).ToString();
        }
    }

    public void BossPoisonHelathbar()
    {
        BossHealthBarFIll.GetComponent<Image>().color = Color.red;
    }
    public void BossNormalHealthBar()
    {
        BossHealthBarFIll.GetComponent<Image>().color = Color.white;
    }
}
