using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [System.Serializable]
    public class MenuItem
    {
        public string menuName;
        public GameObject menuObject;
        [HideInInspector] public CanvasGroup canvasGroup;
    }

    public List<MenuItem> menus;
    public float fadeDuration = 0.4f;

    private string currentMenu;

    private void Awake()
    {
        // اضافه کردن CanvasGroup به همه منوها
        foreach (var item in menus)
        {
            if (item.menuObject.GetComponent<CanvasGroup>() == null)
                item.menuObject.AddComponent<CanvasGroup>();

            item.canvasGroup = item.menuObject.GetComponent<CanvasGroup>();
            item.canvasGroup.alpha = 0;
            item.canvasGroup.interactable = false;
            item.canvasGroup.blocksRaycasts = false;
            item.menuObject.SetActive(false);
        }
    }

    private void Start()
    {
        ShowMenu("Main");
    }

    public void ShowMenu(string name)
    {
        foreach (var item in menus)
        {
            if (item.menuName == name)
            {
                item.menuObject.SetActive(true);
                item.canvasGroup.DOFade(1, fadeDuration);
                item.canvasGroup.interactable = true;
                item.canvasGroup.blocksRaycasts = true;
                currentMenu = name;
            }
            else
            {
                item.canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
                {
                    item.menuObject.SetActive(false);
                });
                item.canvasGroup.interactable = false;
                item.canvasGroup.blocksRaycasts = false;
            }
        }
    }

    public void BackToMain()
    {
        ShowMenu("MainMenu");
    }
}
