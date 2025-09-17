using UnityEngine;

public class CardSoundManager : MonoBehaviour
{
    public static CardSoundManager instance;
    public AudioClip[] bossTakeDamage;
    public AudioSource audioSource;
    private void Awake()
    {
        if(instance == null) { instance =this; }

        audioSource = GetComponent<AudioSource>();  
    }

    public void SetAndPlayAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
    public void BossTakeDamage()
    {
        var randClip=Random.Range(0,bossTakeDamage.Length); 
        audioSource.clip=bossTakeDamage[randClip];
        audioSource.Play();
    }
}
