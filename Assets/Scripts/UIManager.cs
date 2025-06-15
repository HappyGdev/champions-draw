using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject LosePanel;
    public GameObject WinPanel;

    private void Awake()
    {
        if(Instance == null) {Instance = this;}
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
