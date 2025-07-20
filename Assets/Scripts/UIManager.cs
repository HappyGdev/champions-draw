using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject LosePanel;
    public GameObject WinPanel;
    public GameObject BoosApearPanel;
    //public GameObject player_BossTurn;
    public GameObject Player_turn_Over_button;

    public GameObject Playerturn_ui;
    public GameObject BossturnUi;

    public TextMeshProUGUI dmgBoost;

    private void Awake()
    {
        if(Instance == null) {Instance = this;}
    }
    public void PlyerBossTurn(int i)
    {
        //player_BossTurn.GetComponent<TextMeshProUGUI>().text = text;
        if (GameManager.instance.gameOver)
            return;
        StartCoroutine(turnDisplay(i));
    }
    IEnumerator turnDisplay(int turnnum)
    {
        //player turn = 0
        if (turnnum == 0)
        {
            Playerturn_ui.SetActive(true);
            UIAnimationUtility.ShakePosition(Playerturn_ui.GetComponent<RectTransform>(), new Vector3(1, 10, 1), 0.5f, 10, 90, Ease.InOutBounce);
            yield return new WaitForSeconds(1f);
            Playerturn_ui.SetActive(false);
        }
        //boss turn=1
        else if (turnnum == 1) 
        {
            BossturnUi.SetActive(true);
            UIAnimationUtility.ShakePosition(BossturnUi.GetComponent<RectTransform>(), new Vector3(1, 10, 1), 0.5f, 10, 90, Ease.InOutBounce);
            yield return new WaitForSeconds(1f);
            BossturnUi.SetActive(false);
        }

    }
    public void BossPanel(bool stats)
    {
        BoosApearPanel.SetActive(stats);
        UIAnimationUtility.ShakePosition(BoosApearPanel.GetComponent<RectTransform>(), new Vector3(2, 5, 2), 1f, 10, 90, Ease.InOutBounce);
    }
    public void Player_turn_Over_button_On()
    {
        Player_turn_Over_button.SetActive(true);    
    }
    public void Gameover()
    {
        LosePanel.SetActive(true);
    }
    public void Win()
    {
        WinPanel.SetActive(true);
    }

    public void Restart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    public void MiniGame()
    {
        SceneManager.LoadScene(1);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ShowDamageBoost()
    {
        var dmg = CardEffectManager.Instance.CurrentBoost();
        dmgBoost.text = " + " +  dmg.ToString() + " Boost"; 
    }

    private void Update()
    {
        //test score
        if (Input.GetKeyDown(KeyCode.S))
        {
            ScoreManager.Instance.AddScore(10);
        }
    }
}
