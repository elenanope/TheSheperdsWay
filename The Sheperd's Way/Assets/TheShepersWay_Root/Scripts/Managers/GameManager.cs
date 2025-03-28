using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int totalLife = 100;
    public int sheepsAlive;
    BoxCollider2D spawnArea;
    [SerializeField] GameObject sheepPrefab;
    public bool appearingOfSheeps;
    // para cuando haya distintas: public List<GameObject> sheepsPrefabs = new List<GameObject>();
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
        if (totalLife <= 0)
        {
            totalLife = 0;
            currentGameState = GameState.gameOver;
            Debug.Log("No tienes más vida!");
        }
        if (sheepsAlive <= 0)
        {
            sheepsAlive = 0;
            currentGameState = GameState.gameOver;
            Debug.Log("Se han muerto todas las ovejas!");
        }
    }

}
