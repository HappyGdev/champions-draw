using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public static HealthBar instance;

    public Image PlayerhealthBarFill;
    public Image BossHealthBarFIll;
    public float maxHealth = 100f;
    public float PlayercurrentHealth;
    public float BosscurrentHealth;

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
            PlayerhealthBarFill.fillAmount = PlayercurrentHealth / maxHealth;
    }

    public void BossTakeDamage(float amount)
    {
        BosscurrentHealth -= amount;
        BosscurrentHealth = Mathf.Clamp(BosscurrentHealth, 0, maxHealth);
        UIAnimationUtility.ShakePosition(BossHealthBarFIll.rectTransform, new Vector3(1, 10, 1), 0.5f, 10, 90, Ease.InOutBounce);
        UpdateBossHealthUI();
    }

    void UpdateBossHealthUI()
    {
        if (BossHealthBarFIll != null)
            BossHealthBarFIll.fillAmount = BosscurrentHealth / maxHealth;
    }
}
