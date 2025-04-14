using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) Debug.Log("GameManager is missing!");
            return instance;
        }
    }
    public int cruelty = 0;
    public int sheepsAlive;
    BoxCollider2D spawnArea = null;
    [SerializeField] Image crueltyBar;
    [SerializeField] GameObject sheepPrefab;
    [SerializeField] Image fadePanel;
    [SerializeField] PlayerController player;
    public bool appearingOfSheeps;
    // para cuando haya distintas: public List<GameObject> sheepsPrefabs = new List<GameObject>();

    //cruelty points whenever you hit a wolf it increases, different endings for each,
    //if you surpass the limit you lose life (1 heart of love, only have three, and have already a bad ending) 
    public enum GameState { gameOver, gameStarted, gamePaused, gameCompleted }
    public GameState currentGameState = GameState.gameStarted;


    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mantiene el objeto al cambiar de escena
        }
        else Destroy(gameObject); // Si ya hay un GameManager, destruye el nuevo
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnEnable()
    {
        if (appearingOfSheeps)
        {
            spawnArea = GameObject.Find("Area").GetComponent<BoxCollider2D>();

            for (int i = 0; i < sheepsAlive; i++)
            {
                Vector3 colliderCenter = spawnArea.transform.position; 
                Vector3 colliderSize = spawnArea.size; 

                // Generar una posición aleatoria dentro del BoxCollider
                float randomX = Random.Range(colliderCenter.x - colliderSize.x / 2, colliderCenter.x + colliderSize.x / 2);
                float randomY = Random.Range(colliderCenter.y - colliderSize.y / 2, colliderCenter.y + colliderSize.y / 2);

                Vector3 randomPosition = new Vector2(randomX, randomY);

                Instantiate(sheepPrefab, randomPosition, Quaternion.identity);
                Debug.Log("Oveja creada");
            }
        }
    }

    private void Update()
    {
        if(fadePanel == null) fadePanel = GameObject.Find("FadePanel")?.GetComponent<Image>();

        if (crueltyBar != null) crueltyBar.fillAmount = cruelty / 100f;
        else crueltyBar = GameObject.Find("P1crueltyBarFill")?.GetComponent<Image>();
        
        if(player == null) player = GameObject.Find("P1")?.GetComponent<PlayerController>();
        else
        {
            if (player.shepherdLife <= 0 && currentGameState != GameState.gameOver)
            {
                StartCoroutine(LoadLoseScene(4));
                currentGameState = GameState.gameOver;
                Debug.Log("No tienes más vida!");
            }
        }
        
        if (sheepsAlive <= 0 && currentGameState != GameState.gameOver)
        {
            sheepsAlive = 0;
            StartCoroutine(LoadLoseScene(5));
            currentGameState = GameState.gameOver;
            Debug.Log("Se han muerto todas las ovejas!");
        }
    }

    //añadir cercados a final de nivel y que se cierre cuando todas las que queden vivas hayan pasado, sino, solo pasarán esas y no se cerrará

    IEnumerator LoadLoseScene(int sceneToLoad)
    {
        float fadeSpeed = 1f;
        Color actualColor = fadePanel.color;
        actualColor.a = 0f;
        fadePanel.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1.5f);
        while (actualColor.a < 1f)
        {
            actualColor.a += fadeSpeed * Time.deltaTime;
            actualColor.a = Mathf.Clamp01(actualColor.a);

            fadePanel.color = actualColor;

            yield return null;
        }
        SceneManager.LoadScene(sceneToLoad);
        
        
    }
}
