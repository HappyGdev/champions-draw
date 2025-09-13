//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//public class ScoreElement : MonoBehaviour
//{

//    public TMP_Text usernameText;
//    //public TMP_Text killsText;
//    //public TMP_Text deathsText;
//    public TMP_Text scoreText;

//    public void NewScoreElement(string _username, int _score)
//    {
//        usernameText.text = _username;
//        //killsText.text = _kills.ToString();
//        //deathsText.text = _deaths.ToString();
//        scoreText.text = _score.ToString();
//    }

//}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreElement : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text usernameText;
    public TMP_Text scoreText;
    //public TMP_Text profileNumberText;
    public Image avatarImage;

    // New method with all data
    public void NewScoreElement(string username, int score, int profileNumber, int rank)
    {
        rankText.text = $"{rank}";
        usernameText.text = username;
        scoreText.text = score.ToString();
        //profileNumberText.text = $"{profileNumber}";
        avatarImage.sprite = ProfileManager.Instance.avatar[profileNumber];
    }
}

