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
    public GameObject player_BossTurn;
    public GameObject Player_turn_Over_button;

    private void Awake()
    {
        if(Instance == null) {Instance = this;}
    }
    public void PlyerBossTurn_Text(string text)
    {
        player_BossTurn.GetComponent<TextMeshProUGUI>().text = text;
        if (GameManager.instance.gameOver)
            return;
        StartCoroutine(turnTextDisplay());
    }
    IEnumerator turnTextDisplay()
    {
        player_BossTurn.SetActive(true);
        UIAnimationUtility.ShakePosition(player_BossTurn.GetComponent<RectTransform>(), new Vector3(1, 10, 1), 0.5f, 10, 90, Ease.InOutBounce);
        yield return new WaitForSeconds(1f);
        player_BossTurn.GetComponent<TextMeshProUGUI>().text = "";
        player_BossTurn.SetActive(false);
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
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
