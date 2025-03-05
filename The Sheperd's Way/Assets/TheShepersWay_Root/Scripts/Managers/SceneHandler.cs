using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Librería que permite la carga/descarga de escenas
    
public class SceneHandler : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    public int specificSceneToLoad; int sceneToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioSource.Stop();
            SceneManager.LoadScene(specificSceneToLoad);
        }
    }
    public void SceneLoader(int sceneToLoad)
    {
        //audioSource.Stop();
        SceneManager.LoadScene(sceneToLoad);
    }
    public void SceneLoaderOnPlay()
    {
        audioSource.Stop();
        SceneManager.LoadScene(specificSceneToLoad);
    }
    public void ExitGame()
    {
        Application.Quit();
    }

}

