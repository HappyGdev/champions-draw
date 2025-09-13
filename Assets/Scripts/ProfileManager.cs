using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance;
    public List<Sprite> avatar = new List<Sprite>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
}
