using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioSource sfxSource;
    public AudioClip[] sfxList;

    public void PlaySFX(int sfxIndex)
    {
        sfxSource.PlayOneShot(sfxList[sfxIndex]);
    }

}
